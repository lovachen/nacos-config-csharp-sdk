using System;
using System.Collections.Generic;
using System.Text;

namespace NacosConfig.Options
{
    /// <summary>
    /// 发布配置参数
    /// </summary>
    public class PublishParams:Base.BaseParam
    {
        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 配置类型
        /// </summary>
        public string Type { get; set; }
    }
}
