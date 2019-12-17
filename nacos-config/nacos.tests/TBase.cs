using Microsoft.Extensions.DependencyInjection;
using NacosConfig;
using System;
using Xunit;

namespace nacos.tests
{
    public class TBase
    { 
        protected INacosClient _nacosClient;

        public TBase()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddNacos(configure =>
            {  
                configure.VHosts = "http://192.168.0.117:8848";
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _nacosClient = serviceProvider.GetService<INacosClient>(); 
        }
    }
}
