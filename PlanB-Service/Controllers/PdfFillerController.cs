using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlanB_Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PdfFillerController : ControllerBase
    {
        // GET: api/<PdfFillerController>
        [HttpGet]
        public string Get()
        {
            string json = @"
            {
            ""results"": [
                {
                    ""properties"": [
                        {
                            ""label"": ""First name"",
                            ""dataType"": ""STRING"",
                            ""value"": ""Tim Robinson""
                        }
                    ]
                }
            ]
        }";

            return json;
        }


        /*
        // GET api/<PdfFillerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST api/<PdfFillerController>
        [HttpPost]
        public string Post([FromBody] JsonObject customer)
        {
            var body = new StreamReader(Request.Body);

            return "asdsadas" + customer.ToString();
        }

    }
}
