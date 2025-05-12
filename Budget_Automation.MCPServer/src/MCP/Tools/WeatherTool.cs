using Budget_Automation.MCPServer.Infrastructure.Services;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Budget_Automation.MCPServer.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Budget_Automation.MCPServer.Utilities.Helpers;

namespace Budget_Automation.MCPServer.MCP.Tools 
{
    [McpServerToolType]
    public class WeatherTool(
        HttpClient client,
        ILogger<WeatherTool> logger) : HttpBaseService(client, logger)
    {
        private readonly ILogger<WeatherTool> _logger = logger;

        [McpServerTool(Name = "get_weather"), Description("Get the weather information for a specific location.")]
        public async Task<string> GetWeather(
            [Description("The city for which to get the weather information. Use the full name (e.g. Madrid)")] string city)
        {
            try {
                using var jsonDocument = await InvokeServiceAs(HttpMethod.Get, $"https://geocoding-api.open-meteo.com/v1/search?name={city}&count=10&language=en&format=json");
                var json = jsonDocument.RootElement.GetProperty("results")[0];
                if (json.IsNullOrEmpty())
                {
                    return $"No weather information found for {city}.";
                }

                var latitude = json.GetProperty("latitude").GetDouble();
                var longitude = json.GetProperty("longitude").GetDouble();

                var weatherResponse = await InvokeServiceAs(HttpMethod.Get, $"https://api.open-meteo.com/v1/forecast?latitude={latitude.ToInvariantString()}&longitude={longitude.ToInvariantString()}&hourly=temperature_2m&current=temperature_2m,relative_humidity_2m,is_day,precipitation&timezone=auto&forecast_days=1");
                
                return JsonSerializer.Serialize(weatherResponse);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather for {City}", city);
                return JsonSerializer.Serialize(new { error = $"An error  {city} occurred: {ex.Message}," });
            }

        }
    }
}

