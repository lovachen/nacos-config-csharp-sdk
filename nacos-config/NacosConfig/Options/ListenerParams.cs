using NacosConfig.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace NacosConfig.Options
{
    /// <summary>
    /// 监听配置参数
    /// </summary>
    [Serializable]
    public class ListenerParams: BaseParam
    {
        /// <summary>
        /// 锁计数
        /// </summary>
        public int InterNum = 0;

        /// <summary>
        /// 委托回调
        /// </summary>
        public List<Action<string>> Callbacks { get; private set; } = new List<Action<string>>();

        /// <summary>
        /// 添加回调方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ListenerParams Add( Action<string> action)
        {
            Callbacks.Add(action);
            return this;
        }



    }
}
