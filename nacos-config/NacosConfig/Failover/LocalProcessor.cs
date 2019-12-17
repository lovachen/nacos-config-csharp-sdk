using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NacosConfig.Base;
using NacosConfig.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NacosConfig.Failover
{
    /// <summary>
    /// 本地处理配置
    /// </summary>
    public class LocalProcessor : ILocalProcessor
    {
        private readonly static ReaderWriterLockSlim writerLockSlim = new ReaderWriterLockSlim();
        private const string CONFIG_BASE_DIR = "nacos-config";
        private readonly static ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        private ILogger _logger;

        public LocalProcessor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ILocalProcessor>();
        }

        /// <summary>
        /// 获取配置文件内容
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public async Task<string> GetConfigAsync(string dataId, string group, string tenant)
        {
            try
            {
                string config = String.Empty;
                string key = GetCacheKey(dataId, group, tenant);
                if (!_cache.TryGetValue(key, out config))
                {
                    string file_dir = GetFilePath(dataId, group, tenant);
                    var file = new FileInfo(file_dir + dataId);
                    if (!file.Exists)
                        return null;
                    config = File.ReadAllText(file.FullName);
                }
                return await Task.FromResult(config);
            }
            catch (Exception ex)
            {
                _logger.LogError($"读取本地存储配置错误，描述=${ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public async Task RemoveConfigAsync(string dataId, string group, string tenant)
        {
            try
            {
                writerLockSlim.EnterWriteLock();
                string key = GetCacheKey(dataId, group, tenant).ToLower();
                //首次移除
                _cache.Remove(key, out string _);

                string file_dir = GetFilePath(dataId, group, tenant);
                var file = new FileInfo(file_dir);

                if (file.Exists)
                    File.Delete(file.FullName);
                //再次移除
                _cache.Remove(key, out string _);
            }
            catch (Exception ex)
            {
                _logger.LogError($"移除配置时错误，描述={ex.Message}");
            }
            finally
            {
                writerLockSlim.ExitWriteLock();
            }
            await Task.Yield();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task SaveConfigAsync(string dataId, string group, string tenant, string config)
        {
            try
            {
                if (String.IsNullOrEmpty(config))
                    return;
                writerLockSlim.EnterWriteLock();

                string key = GetCacheKey(dataId, group, tenant);
                //首次更新或写入缓存
                _cache.AddOrUpdate(key, config, (k, v) => config);

                string file_dir = GetFilePath(dataId, group, tenant);
                var file = new FileInfo(file_dir);

                //如果我目录不存在则创建
                if (file.Directory != null && !file.Directory.Exists)
                    file.Directory.Create();
                //写入内容
                File.WriteAllText(file.FullName, config);
                //再次更新或写入缓存
                _cache.AddOrUpdate(key, config, (k, v) => config);
            }
            catch (Exception ex)
            {
                _logger.LogError($"写入配置时错误，描述={ex.Message}");
            }
            finally
            {
                if (writerLockSlim.IsWriteLockHeld)
                    writerLockSlim.ExitWriteLock();
            }
            await Task.Yield();
        }


        #region 私有方法

        /// <summary>
        /// 获取配置文件目录
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        private string GetFilePath(string dataId, string group, string tenant)
        {
            string file_dir = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_BASE_DIR, group, dataId);
            if (!String.IsNullOrEmpty(tenant))
                file_dir = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_BASE_DIR, tenant, group, dataId);
            return file_dir;
        }

        /// <summary>
        /// 获取缓存的key
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        private string GetCacheKey(string dataId, string group, string tenant)
        {
            return $"{dataId}-{group}-{tenant}".ToLower();
        }


        #endregion
    }
}
