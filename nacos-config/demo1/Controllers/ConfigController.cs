using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NacosConfig;

namespace demo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private INacosClient _nacosClient;

        public ConfigController(INacosClient nacosClient)
        {
            _nacosClient = nacosClient;
        }

        [Route("pub")]
        public async Task<IActionResult> Pub()
        {

           var res = await _nacosClient.PublishConfigAsync(new NacosConfig.Options.PublishParams()
            {
               DataId= "com.test.1",
               Group= "DEFAULT_GROUP",
               Content="abc2123123131",
               Type= "text",
           });
            return Ok();
        }

        [Route("del")]
        public async Task<IActionResult> Del()
        {
            var res = await _nacosClient.DeleteConfigAsync(new NacosConfig.Options.DeleteParams()
            {
                DataId = "com.test.1",
                Group = "DEFAULT_GROUP", 
            });
            return Ok();
        }
    }
}