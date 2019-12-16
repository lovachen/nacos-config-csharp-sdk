using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NacosConfig.Base;
using NacosConfig.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NacosConfig.Failover
{
    public class LocalProcessor : ILocalProcessor
    {
        public Task<string> GetConfigAsync(string dataId, string group, string tenant)
        {
            throw new NotImplementedException();
        }

        public Task RemoveConfigAsync(string dataId, string group, string tenant)
        {
            throw new NotImplementedException();
        }

        public Task SaveConfigAsync(string dataId, string group, string tenant, string config)
        {
            throw new NotImplementedException();
        }
    }
}
