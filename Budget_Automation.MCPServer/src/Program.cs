using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Microsoft.Extensions.Logging;
using Budget_Automation.MCPServer.Services.Google.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Budget_Automation.MCPServer.MCP.Tools;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"🔧 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔧 IsDebug: {System.Diagnostics.Debugger.IsAttached}");
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure Google Services
builder.Services.Configure<GoogleServiceOptions>(
    builder.Configuration.GetSection("GoogleService"));

builder.Services.AddSingleton<GoogleServiceAccountAuthService>();
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport() 
    .WithTools<GoogleSheetTool>();

builder.Services.AddHttpClient();


var app = builder.Build();

// Validate configuration at startup
try
{
    var googleOptions = app.Services.GetRequiredService<IOptions<GoogleServiceOptions>>().Value;
    if (!googleOptions.IsValid())
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError("GoogleService configuration incomplete");
        Environment.Exit(1);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Configuration error during startup");
    Environment.Exit(1);
}

// Map MCP endpoints
app.MapMcp();

await app.RunAsync();
