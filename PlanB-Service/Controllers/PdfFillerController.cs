using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanB_Service.Models;
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
        [HttpGet("{programa_formularios}/{identificacion_}")]
        public string Get(string programa_formularios, string identificacion_)
        {
            try
            {

                FormFiller formFiller = new FormFiller();

                string folderJsonPath = @"Jsons\planesForm.json";

                Console.WriteLine(folderJsonPath);

                List<string> filesModified = new List<string>();

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

                        List<string> namesFiles = new List<string>();


                        foreach (string propertyName in lista)
                        {

                            if (jsonObject.ContainsKey(propertyName))
                            {
                                // Add the property name and its value to the dictionary
                                Console.WriteLine(jsonObject[propertyName].ToString());

                                namesFiles = JsonConvert.DeserializeObject<List<string>>(jsonObject[propertyName].ToString());

                                foreach (string fileName in namesFiles) {

                                    string fileInputsPath = @"InputFiles\" + fileName + ".pdf";

                                    Console.WriteLine($"Searching: {fileInputsPath}");

                                    if (System.IO.File.Exists(fileInputsPath))
                                    {
                                        Console.WriteLine("Reading: "+ fileInputsPath);

                                        try
                                        {
                                            formFiller.ProcessAsync(fileInputsPath, identificacion_).Wait();

                                            filesModified.Add(fileName);

                                        }
                                        catch  (Exception ex)
                                        {
                                            Console.WriteLine("Error al procesar el archivo: " + fileInputsPath);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("The specified folder does not exist.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Property '{propertyName}' not found in the JSON data.");
                            }
                        }


                    }
                }


                /*
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
                */

                string response = "Formularios procesados correctamente: ";

                foreach (string File in filesModified) {
                    response += File + ", ";
                }

                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        [HttpPost]
        public async Task<string> PostAsync([FromBody] JsonObject customer)
        {

            WebhookBody webhookBody = JsonConvert.DeserializeObject<WebhookBody>(customer.ToString());

            string idTicket = webhookBody.ObjectId.ToString();

            Hubspot.Ticket ticket = new Hubspot.Ticket();

            string ticketData = await ticket.Read(idTicket);

            JObject jsonObject = JObject.Parse(ticketData);

            JObject properties = (JObject)jsonObject["properties"];

            string jsonFilePath = "../Jsons/ticketProps.json";
            string jsonString = File.ReadAllText(jsonFilePath);

            JArray jsonArray = JArray.Parse(jsonString);

            List<string> list = new List<string>();

            // Iterate through each string in the array
            foreach (var propertyName in jsonArray)
            {
                Console.WriteLine((string)properties[propertyName]);
                list.Add((string)properties[propertyName]);
            }


            return (list.ToString());
        }

        /*
        // GET api/<PdfFillerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        */

        /*

        // POST api/<PdfFillerController>
        [HttpPost]
        public string Post([FromBody] JsonObject customer)
        {
            var body = new StreamReader(Request.Body);

            return "asdsadas" + customer.ToString();
        }
        */
    }
}
