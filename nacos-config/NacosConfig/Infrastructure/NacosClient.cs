using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NacosConfig.Base;
using NacosConfig.Failover;
using NacosConfig.Options;
using NacosConfig.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NacosConfig.Infrastructure
{
    /// <summary>
    /// nacos配置客户端
    /// </summary>
    public class NacosClient : INacosClient
    {
        private readonly static Dictionary<string, Timer> listeners = new Dictionary<string, Timer>();
        private ILogger _logger;
        private HttpClient _httpClient;
        private IHttpClientFactory _httpClientFactory;
        private NacosOptions _options;
        private ILocalProcessor _localProcessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="localProcessor"></param>
        /// <param name="loggerFactory"></param>
        public NacosClient(IHttpClientFactory httpClientFactory,
            IOptionsMonitor<NacosOptions> optionsMonitor,
            ILocalProcessor localProcessor,
            ILoggerFactory loggerFactory)
        {
            _localProcessor = localProcessor;
            _options = optionsMonitor.CurrentValue;
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<NacosClient>();
            _httpClient = httpClientFactory.CreateClient(Constant.CLIENT_NAME);
            _httpClient.BaseAddress = new Uri(_options.VHosts);
        }


        /// <summary>
        /// 获取配置 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetConfigAsync(ConfigParams configParams)
        {
            if (configParams == null)
                throw new ArgumentNullException(nameof(configParams));

            //如果本地存在数据，则返回
            var config = await _localProcessor.GetConfigAsync(configParams.DataId, configParams.Group, configParams.Tenant);

            if (!String.IsNullOrEmpty(config))
                return config;
            try
            {
                config = await HttpGetConfigAsync(configParams);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"从nacos服务端获取配置文件失败, dataId={configParams.DataId}, group={configParams.Group}, tenant={configParams.Tenant}, 错误信息={ex.Message}");
            }
            //获取成功 存储到本地
            if (!String.IsNullOrEmpty(config))
            {
                await _localProcessor.SaveConfigAsync(configParams.DataId, configParams.Group, configParams.Tenant, config);
            }
            System.Diagnostics.Debug.WriteLine($"配置={config}");
            return config;
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="listenerParams"></param>
        /// <returns></returns>
        public Task AddListenerAsync(ListenerParams listenerParams)
        {
            if (listenerParams == null)
                throw new ArgumentNullException(nameof(listenerParams));

            string key = $"{listenerParams.DataId}-{listenerParams.Group}-{listenerParams.Tenant}";

            //如果已添加过监听
            if (listeners.ContainsKey(key.ToLower()))
            {
                return Task.CompletedTask;
            }


            Timer timer = new Timer(async o =>
            {
                var p = (ListenerParams)o;
                if (0 == Interlocked.Exchange(ref p.InterNum, 1))
                {
                    await PollingAsync(o);
                    Interlocked.Exchange(ref p.InterNum, 0);
                }
            }, listenerParams, 0, _options.ListenInterval);

            //
            listeners.Add(key.ToLower(), timer);

            return Task.CompletedTask;
        }



        #region private

        /// <summary>
        /// http请求获取配置
        /// </summary>
        /// <param name="configParams"></param>
        /// <returns></returns>
        private async Task<string> HttpGetConfigAsync(ConfigParams configParams)
        {
            string url = $"nacos/v1/cs/configs?dataId={configParams.DataId}&group={configParams.Group}&tenant={configParams.Tenant}";
            var response = await _httpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await response.Content.ReadAsStringAsync();
                case HttpStatusCode.NotFound:
                    await _localProcessor.RemoveConfigAsync(configParams.DataId, configParams.Group, configParams.Tenant);
                    return null;
                case HttpStatusCode.Forbidden:
                    throw new Exception($"Nacos 服务无请求权限。");
                default:
                    throw new Exception($"Nacos 服务错误。状态码:{response.StatusCode}");
            }
        }


        /// <summary>
        /// 监听
        /// </summary>
        /// <returns></returns>
        private async Task PollingAsync(object info)
        {
            var param = (ListenerParams)info;

            //获取本地
            var localConfig = await _localProcessor.GetConfigAsync(param.DataId, param.Group, param.Tenant) ?? "";

            try
            {
                var client = _httpClientFactory.CreateClient(Constant.CLIENT_NAME);
                client.BaseAddress = new Uri(_options.VHosts);
                client.Timeout = TimeSpan.FromSeconds(Constant.LONG_TIMEOUT + 10);

                string one = Char.ConvertFromUtf32(1);
                string tow = Char.ConvertFromUtf32(2);
                string data = String.IsNullOrEmpty(param.Tenant) ? $"{param.DataId}{tow}{param.Group}{tow}{MD5Util.GetMD5(localConfig)}{one}"
                                : $"{param.DataId}{tow}{param.Group}{tow}{MD5Util.GetMD5(localConfig)}{tow}{param.Tenant}{one}";

                HttpContent content = new StringContent($"Listening-Configs={data}");
                content.Headers.TryAddWithoutValidation("Long-Pulling-Timeout", (Constant.LONG_TIMEOUT * 1000).ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await client.PostAsync("nacos/v1/cs/configs/listener", content);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var res = await response.Content.ReadAsStringAsync();
                        if (!String.IsNullOrWhiteSpace(res))
                        {
                            var config = await GetConfigAsync(new ConfigParams() { DataId = param.DataId, Group = param.Group, Tenant = param.Tenant });
                            System.Diagnostics.Debug.WriteLine($"监听配置={config}");
                            await _localProcessor.SaveConfigAsync(param.DataId, param.Group, param.Tenant, config);
                            try
                            {
                                if (param.Callbacks != null)
                                {
                                    param.Callbacks.ForEach(cb =>
                                    {
                                        cb(config);
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"[listener] call back 错误, dataId={param.DataId}, group={param.Group}, tenant={param.Tenant}");
                            }
                        }
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        _logger.LogError($"[listener] 请求无权限, dataId={param.DataId}, group={param.Group}, tenant={param.Tenant}, code={(int)response.StatusCode}");
                        throw new Exception($"Insufficient privilege.");
                    default:
                        _logger.LogError($"[listener] 错误, dataId={param.DataId}, group={param.Group}, tenant={param.Tenant}, code={(int)response.StatusCode}");
                        throw new Exception(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[listener] 错误, dataId={param.DataId}, group={param.Group}, tenant={param.Tenant}, 描述={ex.Message}");
            }

        }










        #endregion













    }
}
