using Microsoft.AspNetCore.Mvc;
using Blog.Attributes;


namespace Blog.Controllers
{

    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {


        [HttpGet]

        [Apikey]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpGet("/config")]

        public  IActionResult GetConfiguration([FromServices] IConfiguration config)
        {
           var env =  config.GetValue<string>("env");
            return Ok(new { 
            mensagem = env});
        }
    }
}
