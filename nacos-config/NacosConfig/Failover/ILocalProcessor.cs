using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NacosConfig.Failover
{
    /// <summary>
    /// 本地处理类
    /// </summary>
    public interface ILocalProcessor
    {
        /// <summary>
        /// 获取配置文件内容
        /// </summary>
        /// <returns></returns>
        Task<string> GetConfigAsync(string dataId, string group, string tenant);


        /// <summary>
        /// 存储配置文件内容
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        Task SaveConfigAsync(string dataId, string group, string tenant, string config);

        /// <summary>
        /// 移除配置本地存储值
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task RemoveConfigAsync(string dataId, string group, string tenant);
    }
}
