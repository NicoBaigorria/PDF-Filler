using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        [HttpGet("{programa_formularios}")]
        public string Get(string programa_formularios)
        {
            try
            {
  
                FormFiller3 formFiller3 = new FormFiller3();

                string folderJsonPath = "C:\\Users\\Usuario\\source\\repos\\PDF-Filler\\PlanB-Service\\Jsons\\planesForm.json";

                Console.WriteLine(folderJsonPath);

                using (StreamReader fileReader = new StreamReader(folderJsonPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(fileReader))
                    {
                        JObject jsonObject = JObject.Load(jsonReader);

                        /*
                        // Get property names
                        IEnumerable<string> propertyNames = jsonObject.Properties().Select(p => p.Name);

                        // Print property names
                        foreach (string propertyName in propertyNames)
                        {
                            Console.WriteLine(propertyName);
                        }
                        */

                        string[] lista = programa_formularios.Split(';');


                        Dictionary<string, object> selectedProperties = new Dictionary<string, object>();

                        foreach (string propertyName in lista)
                        {

                            if (jsonObject.ContainsKey(propertyName))
                            {
                                // Add the property name and its value to the dictionary
                                selectedProperties.Add(propertyName, jsonObject[propertyName]);
                            }
                            else
                            {
                                Console.WriteLine($"Property '{propertyName}' not found in the JSON data.");
                            }
                        }


                    }
                }



                string folderInputsPath = @"C:\Users\Usuario\source\repos\PDF-Filler\PlanB-Service\InputFiles\";

                // Check if the folder exists
                if (Directory.Exists(folderInputsPath))
                {
                    // Get all files with the .pdf extension in the specified folder
                    string[] pdfFiles = Directory.GetFiles(folderInputsPath, "*.pdf");

                    // Iterate through each PDF file
                    foreach (string pdfFile in pdfFiles)
                    {
                        Console.WriteLine("Found PDF file: " + pdfFile);

                       // formFiller3.ProcessAsync(pdfFile);

                    }
                }
                else
                {
                    Console.WriteLine("The specified folder does not exist.");
                }


                return programa_formularios;
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
