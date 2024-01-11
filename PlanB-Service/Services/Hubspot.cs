﻿using RestSharp;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

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


        public async Task UploadFile2(string id, Stream stream) {

            string postUrl = "https://api.hubapi.com/filemanager/api/v3/files/upload?hapikey=demo";
            string filename = "example_file.txt";

            var fileOptions = new
            {
                access = "PUBLIC_INDEXABLE",
                ttl = "P3M",
                overwrite = false,
                duplicateValidationStrategy = "NONE",
                duplicateValidationScope = "ENTIRE_PORTAL"
            };

            var formData = new MultipartFormDataContent
        {
            { "file",  new StreamContent(stream), "uploaded_file"  },
            { "options", new StringContent(JsonConvert.SerializeObject(fileOptions)) },
            { "folderPath", new StringContent("docs") }
        };

            var client = new RestClient();
            var request = new RestRequest(postUrl, Method.Post);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("Content-Disposition", "form-data", ParameterType.RequestBody);
            request.AddParameter("multipart/form-data", formData);

            RestResponse response = client.Execute(request);

            Console.WriteLine($"{response.ErrorException}, {response.StatusCode}, {response.Content}");

        }
        public class Ticket
        {

            public async Task<string> Read(string id)
            {
                List<string> properties = new List<string>() {
                    "age",
                    "identificacion_",
                    "programa_formularios",
                    "apellido",
                    "correo_",
                    "direccion_"
                };

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
