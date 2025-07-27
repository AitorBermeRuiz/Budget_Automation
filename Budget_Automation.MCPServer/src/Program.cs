using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Budget_Automation.MCPServer.MCP.Tools;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Microsoft.Extensions.Logging;
using Budget_Automation.MCPServer.Services.Google.Models;
using Budget_Automation.MCPServer.Utilities.Helpers;

// Manejar comando de autenticación
if (args.Length > 0 && args[0] == "auth")
{
    await AuthenticationHelper.RunAuthenticationFlow();
    return;
}


// Verificar que existan tokens antes de iniciar
AuthenticationHelper.VerifyTokensExist();

// Configuración mínima como en WeatherTool
var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Logging.AddConsole(options => {
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Cargar configuración desde variables de entorno o valores por defecto
var googleSettings = new GoogleApiSettings 
{
    // ClientId y ClientSecret ya no son necesarios aquí - están en el token
    SpreadsheetId = Environment.GetEnvironmentVariable("SPREADSHEET_ID") 
        ?? "TU_SPREADSHEET_ID_POR_DEFECTO",
    TokenPath = Environment.GetEnvironmentVariable("GOOGLE_TOKEN_PATH") 
        ?? "credentials/tokens"
};

// Validar configuración
// {
//     Console.Error.WriteLine(" Error: SPREADSHEET_ID no configurado");
//     Console.Error.WriteLine("   Configura la variable de entorno SPREADSHEET_ID");
//     Environment.Exit(1);
// }

// Registrar servicios esenciales 
builder.Services.AddSingleton(googleSettings);
builder.Services.AddSingleton<GoogleAuthService>();
builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

// Configurar MCP Server 
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<GoogleSheetTool>();

builder.Services.AddHttpClient();

// Construir y ejecutar
var host = builder.Build();
await host.RunAsync();