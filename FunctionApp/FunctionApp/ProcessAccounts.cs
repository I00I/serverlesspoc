// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public static class ProcessAccounts
    {
        [FunctionName("ProcessAccounts")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, [Blob("{data.url}", FileAccess.Read)] Stream input, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
        }
    }
}
