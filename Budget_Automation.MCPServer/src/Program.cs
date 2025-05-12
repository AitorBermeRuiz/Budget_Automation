using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using Microsoft.AspNetCore.Hosting;
using Budget_Automation.MCPServer.MCP.Tools;


var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Configurar servicios de Google Sheets
// builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();
// builder.Services.AddSingleton<ISheetConfigurationService, SheetConfigurationService>();

// Configurar MCP Server con API fluida
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTool>(); // Registramos todas las herramientas del ensamblado

builder.Services.AddHttpClient();

await builder.Build().RunAsync();