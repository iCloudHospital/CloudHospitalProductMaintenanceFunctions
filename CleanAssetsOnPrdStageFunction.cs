using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Web;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.Threading;
using Azure.Storage.Blobs.Models;
using System.Linq;

namespace CloudHospital.ProductMaintenanceFunctions;

public class CleanAssetsOnPrdStageFunction
{
    private readonly ILogger _logger;
    private BlobStorageOptions _blobStorageOptions;

    public CleanAssetsOnPrdStageFunction(
        IOptionsMonitor<BlobStorageOptions> blobStorageOptionsAccessor,
        ILoggerFactory loggerFactory)
    {
        _blobStorageOptions = blobStorageOptionsAccessor.CurrentValue;

        _logger = loggerFactory.CreateLogger<CleanAssetsOnPrdStageFunction>();
    }

    [Function("cleanAssetsOnPrdStageFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var containerName = string.Empty;
        HttpResponseData response;

        try
        {
            if (req.Method.Equals("get", System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(req.Url?.Query))
                {
                    var query = HttpUtility.ParseQueryString(req.Url.Query);
                    containerName = query.Get("containername");
                }
            }

            _logger.LogInformation($"{JsonSerializer.Serialize(_blobStorageOptions)}");

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException("Containername is required", nameof(containerName));
            }
            else
            {
                await ClearAssetsAsync(containerName);
            }

            response = req.CreateResponse(HttpStatusCode.OK);

            response.WriteString($"Ok");
        }
        catch (ArgumentNullException ex)
        {
            response = req.CreateResponse(HttpStatusCode.BadRequest);

            response.WriteString(ex.Message);
        }
        catch (NotFoundException ex)
        {
            response = req.CreateResponse(HttpStatusCode.NotFound);

            response.WriteString(ex.Message);
        }
        catch (Exception ex)
        {
            response = req.CreateResponse(HttpStatusCode.InternalServerError);

            _logger.LogError($"{ex.Message} ${ex}");

            response.WriteString($"Internal Error: {ex.Message}");
        }

        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        return response;
    }

    private async Task ClearAssetsAsync(string containerName, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentNullException("Containername is required", nameof(containerName));
        }

        var serviceClient = new BlobServiceClient(_blobStorageOptions.ConnectionString);

        var blobContainerClient = serviceClient.GetBlobContainerClient(containerName);

        if (!blobContainerClient.Exists())
        {
            throw new NotFoundException("Container does not find");
        }

        var result = blobContainerClient.GetBlobsAsync().AsPages(default, 20);

        // List of blobs in container
        var blobItems = new List<BlobItem>();

        await foreach (Azure.Page<BlobItem> blobPage in result)
        {
            foreach (var blobItem in blobPage.Values)
            {
                blobItems.Add(blobItem);
            }
        }

        _logger.LogInformation($"Total items: {blobItems.Count:n0}; [{string.Join(",", blobItems.Select(x => x.Name))}]");

        // Items to exclude from deletion (Top 3)
        var latestItems = blobItems.OrderByDescending(x => x.Properties.CreatedOn).Take(3).ToList();
        _logger.LogInformation($"Items to exclude: {latestItems.Count:n0}; [{string.Join(",", blobItems.Select(x => x.Name))}]");

        foreach (var item in latestItems)
        {
            var removeCandidate = blobItems.FirstOrDefault(x => x.Name == item.Name);

            if (removeCandidate != null)
            {
                blobItems.Remove(removeCandidate);
            }
        }

        _logger.LogInformation($"Deletion candidate items: {blobItems.Count:n0}; [{string.Join(",", blobItems.Select(x => x.Name))}]");

        if (blobItems.Count > 0)
        {
            foreach (var item in blobItems)
            {
                _logger.LogInformation($"Delete: {item.Name}");
                await blobContainerClient.DeleteBlobIfExistsAsync(item.Name);
            }

            _logger.LogInformation($"Delete {blobItems.Count:n0} item(s).");
        }
        else
        {
            _logger.LogInformation("There are no items to delete.");
        }

    }
}
