using System;
using System.Collections.Generic;
using System.Text;

namespace NacosConfig.Options
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConfigParams
    {
        /// <summary>
        /// 配置 ID。
        /// </summary>
        public string DataId { get; internal set; }

        /// <summary>
        /// 配置分组。
        /// </summary>
        public string Group { get; internal set; }

        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间字段。
        /// </summary>
        public string Tenant { get; internal set; }
    }
}
