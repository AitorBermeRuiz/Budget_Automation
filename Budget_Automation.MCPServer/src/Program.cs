using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Budget_Automation.MCPServer.MCP.Tools;
using Budget_Automation.MCPServer.Services.Google.Models;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;


var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Configurar servicios de Google Sheets
builder.Services.AddSingleton<GoogleAuthService>(); // Registrar GoogleAuthService
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

// Configurar MCP Server con API fluida
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTool>(); // Registramos todas las herramientas del ensamblado

builder.Services.Configure<GoogleApiSettings>(builder.Configuration.GetSection("GoogleApiSettings"));


builder.Services.AddHttpClient();

await builder.Build().RunAsync();