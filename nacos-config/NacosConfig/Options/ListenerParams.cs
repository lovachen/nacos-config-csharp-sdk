using System;
using System.Collections.Generic;
using System.Text;

namespace NacosConfig.Options
{
    /// <summary>
    /// 监听配置参数
    /// </summary>
    [Serializable]
    public class ListenerParams
    {
        /// <summary> 
        /// </summary>
        public string Tenant { get; set; }

        /// <summary> 
        /// </summary>
        public string DataId { get; set; }

        /// <summary> 
        /// </summary>
        public string Group { get; set; }


        /// <summary>
        /// 委托回调
        /// </summary>
        public Action<string> Callback { get; set; }
    }
}
