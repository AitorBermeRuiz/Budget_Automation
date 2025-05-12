using System;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Budget_Automation.MCPServer.Infrastructure.Services;

public class HttpBaseService(HttpClient client, ILogger<HttpBaseService> logger)
{
    private readonly HttpClient _client = client;
    private readonly ILogger<HttpBaseService> _logger = logger;

    protected virtual async Task<JsonDocument> InvokeServiceAs(HttpMethod method, string url, HttpContent? content = null) 
    {
        var request = new HttpRequestMessage(method, url);
        if (content != null)
            request.Content = content;

        try
        {
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Error invoking service at {Url}", url);
            throw;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error invoking service at {Url}", url);
            throw;
        }
    }

    protected virtual async Task<string> InvokeServiceAsObj(HttpMethod method, string url, HttpContent? content = null) 
    {
        var request = new HttpRequestMessage(method, url);
        if (content != null)
            request.Content = content;

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "No data found";
    }

}
