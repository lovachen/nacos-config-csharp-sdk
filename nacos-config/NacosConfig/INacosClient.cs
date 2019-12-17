using NacosConfig.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NacosConfig
{
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
    }
}
