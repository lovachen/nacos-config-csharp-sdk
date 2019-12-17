using NacosConfig.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NacosConfig
{
    /// <summary>
    /// 客户端
    /// </summary>
    public interface INacosClient
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="configParams"></param>
        /// <returns></returns>
        Task<string> GetConfigAsync(ConfigParams configParams);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="listenerParams"></param>
        /// <returns></returns>
        Task AddListenerAsync(ListenerParams listenerParams);
 
        /// <summary>
        /// 发布配置
        /// </summary>
        /// <param name="publishParams"></param>
        /// <returns></returns>
        Task<bool> PublishConfigAsync(PublishParams publishParams);

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="deleteParams"></param>
        /// <returns></returns>
        Task<bool> DeleteConfigAsync(DeleteParams deleteParams);
    }
}
