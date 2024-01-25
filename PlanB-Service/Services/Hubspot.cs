using RestSharp;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using iText.Commons.Utils;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PlanB_Service
{
    internal class Hubspot
    {
        private class ResponseCreateFile
        {

            public string Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool Archived { get; set; }
            public string ParentFolderId { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }


        }
        public async Task<string> CreateFolder(string idFolder, string name)
        {
            var options = new RestClientOptions("https://api.hubapi.com")
            {
                MaxTimeout = -1,
            };

            try
            {
                var client = new RestClient(options);
                var request = new RestRequest("/files/v3/folders", Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
                request.AddParameter("application/json", "{\"parentFolderId\":\"" + idFolder + "\",\"name\":\"" + name + "\"}", ParameterType.RequestBody);
                RestResponse response = await client.ExecuteAsync(request);

                ResponseCreateFile result = JsonConvert.DeserializeObject<ResponseCreateFile>(response.Content);

                Console.WriteLine(result.Name);

                return result.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (ex.Message);
            };

        }

        public async Task UploadFile(string id, string path)
        {

            var options = new RestClientOptions("https://api.hubapi.com")
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/files/v3/files", Method.Post);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
            request.AlwaysMultipartFormData = true;

            // Adjusted file parameter to properly include the file
            request.AddFile("file", path);

            // Adjusted options parameter to use a proper JSON string
            request.AddParameter("folderId", id);
            request.AddParameter("options", "{\"access\": \"PRIVATE\",\"ttl\": \"P2W\",\"overwrite\": false,\"duplicateValidationStrategy\": \"NONE\",\"duplicateValidationScope\":\"EXACT_FOLDER\"}");

            // Use RestResponse<T> for asynchronous execution
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
         
        }

        public async Task DeleteFolder(string idFolder) {
            var options = new RestClientOptions("https://api.hubapi.com")
            {
                MaxTimeout = -1,
            };

            try
            {
                var client = new RestClient(options);
                var request = new RestRequest("/files/v3/folders/"+ idFolder, Method.Delete);
                request.AddHeader("Authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
                RestResponse response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            };
        }

        public async Task ReplaceFile() {
            

        }

        public async Task UploadFile2(string id, Stream stream) {
            Console.WriteLine("Uploaaaaaaaaaaaaaaaaaaaaaaad");

            try
            {

                static byte[] ConvertStreamToBytes(Stream streamData)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        streamData.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }

                byte[] file = ConvertStreamToBytes(stream);

                var fileOptions = new
                {
                    access = "PUBLIC_INDEXABLE"
                };


                var options = new RestClientOptions("https://api.hubapi.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/filemanager/api/v3/files/upload", Method.Post);
                request.AddHeader("Authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
                request.AlwaysMultipartFormData = true;
                request.AddFile("file", file.ToArray(), "FileName.pdf", "application/octet-stream");
                request.AddParameter("options", JsonConvert.SerializeObject(fileOptions));
                
               request.AddParameter("folderPath", "/PDF-Gobierno_de_Canada/PDF-API/pruebadasdasdas");

                RestResponse response = await client.ExecuteAsync(request);
                Console.WriteLine("SIIIIIIIIIIIIIII-----"+ response.Content);

            }
            catch (Exception ex) { 
            
                   Console.WriteLine("ERRROOOOOOOOOOOOOOOR",ex.ToString());
            }

        }

        public async Task GetFile() {
            var client = new RestClient("https://api.hubapi.com/files/v3/files/");
            var request = new RestRequest("https://api.hubapi.com/files/v3/files/", Method.Get);
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
            RestResponse response = client.Execute(request);
        }

        
        public class Ticket
        {

            public async Task<string> Read(string id)
            {

                List<string> properties = new List<string>();

                string JsonTicketProps = @"Jsons/ticketProps.json";

                string jsonContent = System.IO.File.ReadAllText(JsonTicketProps);

                JObject jsonObjectProps = JObject.Parse(jsonContent);

                JToken propertyValue = jsonObjectProps["props"];

                if (propertyValue != null && propertyValue.Type == JTokenType.Array)
                {
                    properties = propertyValue.ToObject<List<string>>();
                }

                string stringProperties = "";

                foreach (string property in properties)
                {
                    stringProperties += "properties=" + property + "&";
                }

                var client = new RestClient("https://api.hubapi.com/crm/v3/objects/tickets/" + id + "?"+ stringProperties + "archived=false");
                var request = new RestRequest("", Method.Get);
                request.AddHeader("accept", "application/json");
                request.AddHeader("authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
                RestResponse response = await client.ExecuteAsync(request);


                Console.Write("Respuesta Ticket Data:" + response.Content);

                return (response.Content);
            }

            public async Task UpdateProperties(string id, string properties)
            {
                var options = new RestClientOptions("https://api.hubapi.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/crm/v3/objects/tickets/" + id, Method.Patch);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("Authorization", "Bearer pat-na1-31886066-9adb-4992-930a-91cd28f192ff");
                var body = "{\"properties\": " + properties + "}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);
                Console.WriteLine("Propiedades actualizadas// " + body);
            }

        }
    }


}
