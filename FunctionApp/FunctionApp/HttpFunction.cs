using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FunctionApp
{
    public static class HttpFunction
    {
        //private const string connString =
        //    "DefaultEndpointsProtocol=https;AccountName=storeless;AccountKey=0apWHYI0kQsnkI9mYKsrHyYxV2LaDYpho29E1qptj7OuUkxJTV5mR8xuJTg3EuD58F/RZrKQkr1EQo058QsbEQ==;BlobEndpoint=https://storeless.blob.core.windows.net/;TableEndpoint=https://storeless.table.core.windows.net/;QueueEndpoint=https://storeless.queue.core.windows.net/;FileEndpoint=https://storeless.file.core.windows.net/";

        [FunctionName("HttpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmId = "a9051d9c-787e-41a6-9eb6-0bf674b9ffce";
            var engagementId = "d7b66390-0465-422f-a7e6-27c8feecf2b4";

            log.LogInformation("Function triggered");
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            string filename = req.Query["filename"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var rootObject = JsonConvert.DeserializeObject<RootObject>(requestBody);
            var url = rootObject?.Body?.Data?.Url;
            filename = filename ?? url.Substring(url.IndexOf(firmId) + firmId.Length + 1);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(firmId);
            var blockBlobReference = container.GetBlockBlobReference(filename);
            log.LogInformation($"processing {filename}");
            var extractedAccounts = new List<ExtractedAccount>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await blockBlobReference.DownloadToStreamAsync(stream);

                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
                    {
                        var workbookPart = doc.WorkbookPart;
                        var worksheetPart = workbookPart.WorksheetParts.First();
                        var sheet = worksheetPart.Worksheet;
                        var rows = sheet.Descendants<Row>().ToList();

                        log.LogInformation($"{rows.LongCount()} accounts in {filename}");


                        for (var i = 1; i < rows.Count(); i++)
                        {
                            extractedAccounts.Add(new ExtractedAccount
                            {
                                Id = Guid.NewGuid().ToString("n"),
                                Name = rows.ElementAt(i).ChildElements.ElementAt(0).InnerText,
                                Number = rows.ElementAt(i).ChildElements.ElementAt(1).InnerText
                            });

                        }
                    }
                }

                var accountContainer = blobClient.GetContainerReference(firmId);
                var blob = accountContainer.GetBlockBlobReference(engagementId + "/accounts/accounts.json");
                blob.Properties.ContentType = "application/json";

                using (var accountStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(extractedAccounts))))
                {
                    await blob.UploadFromStreamAsync(accountStream);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"error processing {filename}");
                throw;
            }

            return new OkObjectResult($"Completed processing {filename}");
        }


        private static string GetBlobNameFromUrl(string bloblUrl)
        {
            var uri = new Uri(bloblUrl);
            var cloudBlob = new CloudBlob(uri);
            return cloudBlob.Name;
        }
    }
}