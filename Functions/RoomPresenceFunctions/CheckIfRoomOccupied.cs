using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace RoomPresenceFunctions
{
    public static class CheckIfRoomOccupied
    {
        [FunctionName("CheckIfRoomOccupied")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "HttpTriggerCSharp/name/{name}")]HttpRequestMessage req,
            string name,
            [Table("RoomOccupation")] CloudTable allRoomStatus,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var status = allRoomStatus.CreateQuery<RoomStatus>()
                .Where(s => s.PartitionKey == name)
                .FirstOrDefault();

            string returnStatus = "unknown";

            if (status != null)
            {
                returnStatus = status.IsOccupied == true ? "occupied" : "unoccupied";
            }

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, returnStatus);
        }
    }
}
