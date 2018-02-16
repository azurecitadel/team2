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

            using (HttpClient client = new HttpClient())
            {
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

                // Expect the blob name format to be Location.RoomName.Date.Time.jpg or similar
                var splitName = name.Split('.');
                var roomId = $"{splitName[0]}.{splitName[1]}";

                //https://team2iotfuncstorage.blob.core.windows.net/samples-workitems/TVP_B1.Kelvin.20180117.1746.jpg

                return new RoomStatus
                {
                    PartitionKey = roomId,
                    RowKey = GenerateLogTailRowKey(),
                    IsOccupied = personPresent,
                    ImageUrl = $"https://team2iotfuncstorage.blob.core.windows.net/samples-workitems/{name}"
                };
            }
        }

        private static string GenerateLogTailRowKey()
        {
            return string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
        }
    }
}
