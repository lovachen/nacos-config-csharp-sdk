using Microsoft.Extensions.DependencyInjection;
using NacosConfig.Options;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using NacosConfig.Base;
using Microsoft.Extensions.Configuration;
using NacosConfig.Failover;
using NacosConfig;
using NacosConfig.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Nacos DI 注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacos(this IServiceCollection services, Action<NacosOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.Configure(configure);
            services.AddHttpClient(Constant.CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false }); 
            services.AddSingleton<INacosClient, NacosClient>();
            services.AddSingleton<ILocalProcessor, LocalProcessor>();

            return services;
        } 


        /// <summary>
        /// Nacos DI 注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacos(this IServiceCollection services, IConfiguration configuration, string sectionName = "nacos")
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<NacosOptions>(configuration.GetSection(sectionName));
            services.AddHttpClient(Constant.CLIENT_NAME)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { UseProxy = false }); 
            services.AddSingleton<ILocalProcessor, LocalProcessor>();
            services.AddSingleton<INacosClient, NacosClient>();

            return services;
        }


        /// <summary>
        /// 应用启动时注册监听
        /// 也可以不使用此方法，手动去注册监听
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UserNacosListening(this IApplicationBuilder app,List<ListenerParams> options)
        {
            if (options == null || !options.Any())
                throw new ArgumentNullException(nameof(options));

            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var nacosClient = app.ApplicationServices.GetRequiredService<INacosClient>();
            //启动程序时开启监听
            lifetime.ApplicationStarted.Register(() =>
            {
                foreach(var op in options)
                {
                    nacosClient.AddListenerAsync(op);
                }
            });
            return app;
        }





    }
}
