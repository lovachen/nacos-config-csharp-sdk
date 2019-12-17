using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace nacos.tests
{
     public class ConfigTest:TBase
    {
        [Fact]
        public async Task GetConfig()
        {
           var cf = await _nacosClient.GetConfigAsync(new NacosConfig.Options.ConfigParams()
            {
                DataId= "com.user.servier.api",
                Group= "DEFAULT_GROUP",
                Tenant = "",
            });
            Assert.NotNull(cf);
        }
    }
}
