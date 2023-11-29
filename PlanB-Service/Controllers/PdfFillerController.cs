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
            try
            {
                string json = @"{
  ""results"": [
    {
      ""objectId"": 245,
      ""title"": ""API-22: APIs working too fast"",
      ""link"": ""http://example.com/1"",
      ""created"": ""2016-09-15"",
      ""priority"": ""HIGH"",
      ""project"": ""API"",
      ""reported_by"": ""msmith@hubspot.com"",
      ""description"": ""Customer reported that the APIs are just running too fast. This is causing a problem in that they're so happy."",
      ""reporter_type"": ""Account Manager"",
      ""status"": ""In Progress"",
      ""ticket_type"": ""Bug"",
      ""updated"": ""2016-09-28"",
     
    },
]
    }";

                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

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
