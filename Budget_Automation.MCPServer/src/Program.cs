using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Budget_Automation.MCPServer.MCP.Tools;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Microsoft.Extensions.Logging;
using Budget_Automation.MCPServer.Services.Google.Models;

// Configuración mínima como en WeatherTool
var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Logging.AddConsole(options => {
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Registrar servicios esenciales 
var googleSettings = new GoogleApiSettings 
{
    ClientId = "435059752038-6qn8phbngtj2781v3d8hb3fbb8li0smu.apps.googleusercontent.com",
    ClientSecret = "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",
    SpreadsheetId = "fdassd"
};
builder.Services.AddSingleton(googleSettings);
builder.Services.AddSingleton<GoogleAuthService>();
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

// Configurar MCP Server 
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<GoogleSheetTool>();

// Agregar HttpClient si es necesario
builder.Services.AddHttpClient();

// Construir y ejecutar
var host = builder.Build();
await host.RunAsync();