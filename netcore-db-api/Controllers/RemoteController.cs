// 创建人：taomee
// 创建时间：2020/06/08 17:31

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using netcore_db_api.Consul;

namespace netcore_db_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemoteController : ControllerBase
    {

//        [HttpGet]
        public string Get()
        {
            //向服务发送请求
            using (var httpClient = new HttpClient())
            {
                var service = ConsulProxyUtil.GetRandomSerice("DemoService");
                var result = httpClient.GetAsync($"http://{service.Address}:{service.Port}/HealthCheck");
                Console.WriteLine($"调用{service.Service}，状态：{result.Result.StatusCode}");
                return service.Service + "-" + result.Result.StatusCode;
            }
        }
    }
}