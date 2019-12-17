using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NacosConfig.Options;

namespace demo1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNacos(Configuration);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UserNacos(new List<ListenerParams>()
            {
                new ListenerParams()
                {
                    DataId="com.user.servier.api",
                    Group="DEFAULT_GROUP",
                    //Tenant="",
                }.Add(x=>{
                    System.Diagnostics.Debug.WriteLine($"call back 1============{x}");
                }).Add(x =>
                {
                    System.Diagnostics.Debug.WriteLine($"call back 2============{x}");
                }),
                new ListenerParams()
                {
                    DataId="com.api.order.json",
                    Group="DEFAULT_GROUP",
                    //Tenant="",
                }.Add(x =>
                {
                    System.Diagnostics.Debug.WriteLine($"call back 1 {x}");
                })
            });
        }
    }
}
