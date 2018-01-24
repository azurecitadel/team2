using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace RoomPresenceFunctions
{
    public static class CheckRoomOccupationByName
    {
        [FunctionName("CheckRoomOccupationByName")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "CheckRoomOccupationByName/{name}")]HttpRequestMessage req,
            string name,
            [Table("RoomOccupation")] CloudTable allRoomStatus,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            RoomStatus lastStatus = allRoomStatus.CreateQuery<RoomStatus>()
                .Where(s => s.PartitionKey == name)
                .FirstOrDefault();

            string statusString = "unknown";
            string timeStamp = "unavailable";
            string imageUrl = "unavailable";

            if (lastStatus != null)
            {
                statusString = lastStatus.IsOccupied == true ? "occupied" : "unoccupied";
                timeStamp = lastStatus.Timestamp.UtcDateTime.ToString();
                imageUrl = lastStatus.ImageUrl;
            }

            string statusJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    RoomName = name,
                    OccupiedStatus = statusString,
                    TimeStamp = timeStamp,
                    ImageUrl = imageUrl
                });

            // Fetching the name from the path parameter in the request URL

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(statusJson, Encoding.UTF8, "application/json") };

            //return req.CreateResponse(HttpStatusCode.OK, new StringContent(statusJson, Encoding.UTF8, "application/json"));
        }
    }
}
