using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace netcore_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
       

        private readonly ILogger<IndexController> _logger;

        public IndexController(ILogger<IndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello,DotNet Core Api";
        }
    }
}