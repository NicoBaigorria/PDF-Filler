using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanB_Service.Models;
using System;
using System.Net.Http.Json;
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

        [HttpPost]
        public async Task<string> PostAsync([FromBody] JsonObject customer)
        {


            List<string> filesModified = new List<string>();

            async Task<string> CreatePdf(string programa_formularios, string identificacion_, string properties)
            {

                string listaPdfEncontrados = "lista: ";


                string listaPdfsInput = "";


                try
                {

                    FormFiller formFiller = new FormFiller();

                    string folderJsonPath = @"Jsons/planesForm.json";

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

                            string[] filePaths = Directory.GetFiles(@"InputFiles/", "*.pdf", SearchOption.TopDirectoryOnly);

                            Dictionary<string, object> selectedProperties = new Dictionary<string, object>();

                            List<string> namesFiles = new List<string>();

                            foreach (string namePdf in filePaths)
                            {
                                listaPdfsInput += namePdf +", ";
                            }

                            foreach (string propertyName in lista)
                            {

                                if (jsonObject.ContainsKey(propertyName))
                                {
                                    // Add the property name and its value to the dictionary
                                    Console.WriteLine(jsonObject[propertyName].ToString());

                                    namesFiles = JsonConvert.DeserializeObject<List<string>>(jsonObject[propertyName].ToString());

                                    foreach (string fileName in namesFiles)
                                    {

                                        string fileInputsPath = @"InputFiles/" + fileName + ".pdf";

                                        Console.WriteLine($"Searching: {fileInputsPath}");

                                        listaPdfEncontrados += fileInputsPath + ":";


                                        if (System.IO.File.Exists(fileInputsPath))
                                        {
                                            Console.WriteLine("Reading: " + fileInputsPath);

                                            listaPdfEncontrados += "encontrado";

                                            try
                                            {
                                                formFiller.ProcessAsync(fileInputsPath, identificacion_, properties).Wait();

                                                filesModified.Add(fileName);

                                            }
                                            catch (Exception ex)
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
                    string folderInputsPath = @"C:/Users/Usuario/source/repos/PDF-Filler/PlanB-Service/InputFiles/";

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

                    foreach (string File in filesModified)
                    {
                        response += File + ", ";
                    }

                    string url = identificacion_;

                    return listaPdfsInput;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            // Obtener Body

            WebhookBody webhookBody = JsonConvert.DeserializeObject<WebhookBody>(customer.ToString());

            string idTicket = webhookBody.ObjectId.ToString();

            // Crear TicketData a partir del post

            Hubspot.Ticket ticket = new Hubspot.Ticket();

            string ticketData = await ticket.Read(idTicket);

            JObject jsonObject = JObject.Parse(ticketData);

            // Optener propiedades del contacto a partir del Ticket

            JObject properties = (JObject)jsonObject["properties"];

            /*
            string jsonFilePath = "./Jsons/ticketProps.json";
            string jsonString = System.IO.File.ReadAllText(jsonFilePath);

            JObject json = JObject.Parse(jsonString);

            Console.WriteLine(json["props"]);
            */

            // Traer lista de propiedades utiles para el formulario

            List<string> list = new List<string> { };

            List<string> propertiesList = new List<string>();

            string JsonTicketProps = @"Jsons/ticketProps.json";

            string jsonContent = System.IO.File.ReadAllText(JsonTicketProps);

            JObject jsonObjectProps = JObject.Parse(jsonContent);

            JToken propertyValue = jsonObjectProps["props"];

            if (propertyValue != null && propertyValue.Type == JTokenType.Array)
            {
               propertiesList = propertyValue.ToObject<List<string>>();
            }

                // Agregar Lista de valores 
                foreach (string propertyName in propertiesList)
            {
                try
                {
                    Console.WriteLine((string)properties[propertyName]);
                    list.Add((string)properties[propertyName]);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            static string FilterJsonProperties(string jsonString, List<string> propertiesToFilter)
            {
                // Parse the JSON string into a JObject
                JObject jsonObject = JObject.Parse(jsonString);

                // Create a new JObject with only the specified properties
                JObject filteredObject = new JObject();
                foreach (string property in propertiesToFilter)
                {
                    if (jsonObject.TryGetValue(property, out var value))
                    {
                        filteredObject[property] = value;
                    }
                }

                // Convert the filtered JObject back to a JSON string
                string filteredJson = filteredObject.ToString();

                return filteredJson;
            }

            // Filtrar propiedades
            string filteredJson = FilterJsonProperties(properties.ToString(), propertiesList);

            // Crear Carpeta

            Hubspot hubspot = new Hubspot();

            string idFolder = await hubspot.CreateFolder("145506339115", (string)properties["identificacion_"]);

            string jsonProperties = "{\"id_folder\": " + idFolder + "}";

            await ticket.UpdateProperties(idTicket, jsonProperties);

            // Crer Pdf en Hubspot

            string responsePdf = await CreatePdf((string)properties["programa_formularios"], (string)properties["identificacion_"], filteredJson);


            return (responsePdf);
        }

    }
}
