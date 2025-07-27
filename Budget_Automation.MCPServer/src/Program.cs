using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Microsoft.Extensions.Logging;
using Budget_Automation.MCPServer.Services.Google.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Logging.AddConsole(options => {
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Registrar servicios esenciales 
builder.Services.Configure<GoogleServiceOptions>(builder.Configuration.GetSection("GoogleService"));

builder.Services.AddSingleton<GoogleServiceAccountAuthService>();
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

#if DEBUG
// 🔧 MODO DEBUG: Sin MCP, solo testing
builder.Services.AddHttpClient();

var host = builder.Build();

// Validar configuración
try
{
    var googleOptions = host.Services.GetRequiredService<IOptions<GoogleServiceOptions>>().Value;
    if (!googleOptions.IsValid())
    {
        Console.Error.WriteLine("❌ GoogleService configuration incomplete.");
        Environment.Exit(1);
    }

    // Test de conexión
    var sheetsService = host.Services.GetRequiredService<IGoogleSheetsService>();
    var testResult = await sheetsService.ReadRange(new List<string> { "A1:A1" });

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"❌ Error: {ex.Message}");
    Environment.Exit(1);
}

#else
// Configurar MCP Server 
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<GoogleSheetTool>();

builder.Services.AddHttpClient();

// Construir y ejecutar
var host = builder.Build();
await host.RunAsync();
#endif