using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Budget_Automation.MCPServer.MCP.Tools;
using Budget_Automation.MCPServer.Services.Google.Models;
using Budget_Automation.MCPServer.Services.Google;
using Budget_Automation.MCPServer.Services.Google.Abstract;

var builder = Host.CreateDefaultBuilder(args);

// Configurar servicios de Google Sheets
builder.ConfigureServices((context, services) =>
{
    // Registrar servicios
    services.AddSingleton<GoogleAuthService>();
    services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

    // Configurar MCP Server con API fluida
    services
        .AddMcpServer()
        .WithStdioServerTransport()
        .WithTools<GoogleSheetTool>();

    // Configurar GoogleApiSettings
    var xx = context.Configuration.GetSection("GoogleApiSettings");
    services.Configure<GoogleApiSettings>(xx);
    services.AddHttpClient();
});

var app = builder.Build();

// Llamar a los métodos de GoogleSheetsService con datos reales
// using (var scope = app.Services.CreateScope())
// {
//     var googleSheetsService = scope.ServiceProvider.GetRequiredService<IGoogleSheetsService>();

//     // Probar el método ReadRange
//     try
//     {
//         Console.WriteLine("Probando el método ReadRange...");
//         var ranges = new List<string> { "Presupuesto!A1:B10", "Dashboard!A1:D10" };
//         var response = await googleSheetsService.ReadRange(ranges);

//         foreach (var valueRange in response.ValueRanges)
//         {
//             Console.WriteLine($"Rango: {valueRange.Range}");
//             foreach (var row in valueRange.Values)
//             {
//                 Console.WriteLine(string.Join(", ", row));
//             }
//         }
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Error al probar ReadRange: {ex.Message}");
//     }
//     // Probar el método UpdateRange
//     try
//     {
//         Console.WriteLine("Probando el método UpdateRange...");
//         var range = "Gastos!A6:E7";
//         var values = new List<IList<object>>
//         {
//             new List<object> { "22/12/2024", "Ingreso", "Ocio", 12, "prueba 1" },
//             new List<object> { "22/12/2025", "Gasto", "Transporte", 200, "prueba 2"  }
//         };

//         var updateResponse = await googleSheetsService.UpdateRange(range, values);
//         Console.WriteLine($"Rango actualizado: {updateResponse.UpdatedRange}");
//         Console.WriteLine($"Filas actualizadas: {updateResponse.UpdatedRows}");
//         Console.WriteLine($"Columnas actualizadas: {updateResponse.UpdatedColumns}");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Error al probar UpdateRange: {ex.Message}");
//     }
// }

await app.RunAsync();