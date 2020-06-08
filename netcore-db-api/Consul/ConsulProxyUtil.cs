// 创建人：taomee
// 创建时间：2020/06/08 17:45

using System;
using System.Collections.Generic;
using System.Linq;
using Consul;

namespace netcore_db_api.Consul
{
    public class ConsulProxyUtil
    {
        private const string ConsulUri = "http://127.0.0.1:8500";
        
        public static IEnumerable<AgentService> GetAllSerices()
        {
            using (var consul = new ConsulClient(c =>
            {
                c.Address = new Uri(ConsulUri);
            }))
            {
                //取在Consul注册的全部服务
                var services = consul.Agent.Services().Result.Response;
                foreach (var s in services.Values)
                {
                    Console.WriteLine($"ID={s.ID},Service={s.Service},Addr={s.Address},Port={s.Port}");
                }

                return services.Values;
            }
        }
        
        
        public static IEnumerable<AgentService> GetAllSerices(string serviceName)
        {
            using (var consul = new ConsulClient(c =>
            {
                c.Address = new Uri(ConsulUri);
            }))
            {
                //取在Consul注册的全部名为serviceName的服务
                var services = consul.Agent.Services().Result.Response;
                return services.Values.Where(p => p.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }
        }
        
        public static AgentService GetRandomSerice(string serviceName)
        {
            using (var consul = new ConsulClient(c =>
            {
                c.Address = new Uri(ConsulUri);
            }))
            {
                //取在Consul注册的全部名为serviceName的服务
                var nodes = consul.Agent.Services().Result.Response;
                var services = nodes.Values.Where(p => p.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                //客户端负载均衡，随机选出一台服务
                var rand = new Random();
                var index = rand.Next(services.Count());
                return services.ElementAt(index);
            }
        }
        
    }
}