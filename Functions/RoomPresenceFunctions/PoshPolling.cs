using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace RoomPresenceFunctions
{
    public static class PoshPolling
    {
        [FunctionName("PoshPolling")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            const string uriBase = "https://team2iot.azurewebsites.net/api/";

            var functionKey = System.Environment.GetEnvironmentVariable("FunctionKey", System.EnvironmentVariableTarget.Process);

            using (HttpClient client = new HttpClient())
            {
                log.Info("Polling occupancy function...");

                var response = client.GetAsync($"{uriBase}CheckRoomOccupationByName/TVP_B1.Kelvin?code={functionKey}").GetAwaiter().GetResult();

                log.Info($"Polled occupancy function. Response: {response.StatusCode}");
            }
        }
    }
}
