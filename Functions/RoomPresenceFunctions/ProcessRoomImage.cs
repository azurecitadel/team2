using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RoomPresenceFunctions
{
    public static class ProcessRoomImage
    {
        const float ConfidenceThreshold = 0.75F;

        [FunctionName("ProcessRoomImage")]
        [return: Table("RoomOccupation")]
        public async static Task<RoomStatus> Run([BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var subscriptionKey = System.Environment.GetEnvironmentVariable("CognitiveServicesApiKey", System.EnvironmentVariableTarget.Process);
            //log.Info($"CS Key: {subscriptionKey}");

            const string uriBase = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/tag";

            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            //string requestParameters = "visualFeatures=Categories,Description,Color&language=en";

            // Assemble the URI for the REST API Call.
            string uri = uriBase; //+ "?" + requestParameters;

            HttpResponseMessage response;
            bool personPresent = false;

            using (StreamContent content = new StreamContent(myBlob))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                string responseString = await response.Content.ReadAsStringAsync();

                JObject tags = JObject.Parse(responseString);
                JToken personTag = tags.SelectToken("$.tags[?(@.name == 'person')]");

                if (personTag != null)
                {
                    float personConfidence = (float)personTag["confidence"];
                    personPresent = personConfidence > ConfidenceThreshold ? true : false;
                }
            }


            // Expect the blob name format to be RoomName.jpg or similar
            var roomId = name.Split('.')[0];

            return new RoomStatus { PartitionKey = roomId, RowKey = Guid.NewGuid().ToString(), IsOccupied = personPresent };
        }
    }
}
