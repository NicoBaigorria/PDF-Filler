using iText.StyledXmlParser.Jsoup.Nodes;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Filler.Services
{
    internal class Hubspot
    {

        public async Task uploadFileAsync()
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
            request.AddFile("file", @"C:\Users\nicob\OneDrive\Documentos\GitHub\PDF-Filler\PDF-Filler\InputFiles\imm1294e.pdf");

            // Adjusted options parameter to use a proper JSON string
            request.AddParameter("folderId", "145506339115");
            request.AddParameter("options", "{\"access\": \"PRIVATE\",\"ttl\": \"P2W\",\"overwrite\": false,\"duplicateValidationStrategy\": \"NONE\",\"duplicateValidationScope\":\"EXACT_FOLDER\"}");

            // Use RestResponse<T> for asynchronous execution
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);

        }
        }
    }
