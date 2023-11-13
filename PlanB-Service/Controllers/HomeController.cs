using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlanB_Service.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: />
        [HttpGet]
        public string Get()
        {
            return "ok";
        }
    }
}
