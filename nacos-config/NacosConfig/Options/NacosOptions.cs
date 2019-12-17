using System;
using System.Collections.Generic;
using System.Text;

namespace NacosConfig.Options
{
    /// <summary>
    /// 配置
    /// </summary>
    [Serializable]
    public class NacosOptions
    {
        /// <summary>
        /// 虚拟主机地址，可带端口，例如：http://192.168.1.100:8848
        /// 一般是Nginx的配置地址
        /// </summary>
        public string VHosts { get; set; }

        /// <summary>
        /// 配置监听间隔 毫秒为单位 默认1000ms
        /// </summary>
        public int ListenInterval { get; set; } = 10000;
    }
}
