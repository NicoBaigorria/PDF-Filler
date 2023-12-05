using RestSharp;
using Newtonsoft.Json;

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
            }catch(Exception ex)
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
    }
}
