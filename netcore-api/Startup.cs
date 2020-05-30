using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace netcore_api
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // var ip = Configuration["ip"];
            // var port = Configuration["port"];
            var url = Configuration["urls"];
            var ipStart = url.IndexOf("http://") + 7;
            var ipEnd = url.LastIndexOf(":");
            var ipLength = ipEnd - ipStart;
            var ip = url.Substring(ipStart, ipLength);
            var port = url.Substring(ipEnd + 1);
            var serviceName = "DemoService";
            var serviceId = serviceName + Guid.NewGuid();
            //注册服务到Consul
            using (var consulClient = new ConsulClient(ConsulConfig))
            {
                var registration = new AgentServiceRegistration();
                //服务提供方的ip和端口
                registration.Address = ip;
                registration.Port = Convert.ToInt32(port);
                //服务编号
                registration.ID = serviceId;
                //服务名称
                registration.Name = serviceName;
                //健康检查地址
                registration.Check = new AgentServiceCheck()
                {
                    //服务停止多久后注销
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    //健康检查地址
                    HTTP = $"http://{ip}:{port}/HealthCheck",
                    //心跳间隔时间
                    Interval = TimeSpan.FromSeconds(10),
                    //请求超时时间
                    Timeout = TimeSpan.FromSeconds(5)
                };
                consulClient.Agent.ServiceRegister(registration).Wait();
            }

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                using (var consulClient = new ConsulClient(ConsulConfig))
                {
                    Console.WriteLine($"服务注销, time : {DateTime.Now}");
                    consulClient.Agent.ServiceDeregister(serviceId).Wait();
                }
            });
        }

        private void ConsulConfig(ConsulClientConfiguration config)
        {
            //配置注册中心地址
            config.Address = new Uri("http://localhost:8500");
            config.Datacenter = "dc1";
        }
    }
}