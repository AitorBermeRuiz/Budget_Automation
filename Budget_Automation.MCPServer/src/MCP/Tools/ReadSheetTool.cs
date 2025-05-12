// namespace Budget_Automation.MCPServer.Tools
// {
//     using System.Collections.Generic;
//     using System.ComponentModel;
//     using System.Threading.Tasks;
//     using Budget_Automation.MCPServer.Services.Google.Abstract;
//     using Microsoft.Extensions.Configuration;
//     using Microsoft.Extensions.Logging;
//     using ModelContextProtocol.Server;

//     [McpServerToolType]
//     public class ReadSheetTool
//     {
//         private readonly IGoogleSheetsClient _googleSheetsClient;
//         private readonly ILogger<ReadSheetTool> _logger;

//         public ReadSheetTool(
//             IGoogleSheetsClient googleSheetsClient,
//             ILogger<ReadSheetTool> logger)
//         {
//             _googleSheetsClient = googleSheetsClient;
//             _logger = logger;
//         }

//         [McpServerTool(
//             Name= "range", 
//             Title = "Lee datos de una hoja de cálculo de Google Sheets especificando un rango. Puede leer gastos, presupuestos o cualquier otro rango de datos."
//             ), Description("Lee datos de una hoja de cálculo de Google Sheets especificando un rango. Puede leer gastos, presupuestos o cualquier otro rango de datos.")]
//         public async Task<IMcpToolResult> ExecuteAsync([Description("indica el rango de la hoja de cálculo de Google Sheets a leer. Ejemplo: 'Hoja1!A1:B10'")]string range)
//         {
//             try
//             {
//                 // Validar que se proporcionó un rango
//                 if (string.IsNullOrWhiteSpace(range))
//                 {
//                     return MessageType.Error($"El rango no puede estar vacío o nulo.");
//                 }

//                 // Leer los datos del rango especificado
//                 var data = await _googleSheetsClient.ReadRangeAsync(Range);

//                 // Si no hay datos, informar de manera amigable
//                 if (data == null || data.Count == 0)
//                 {
//                     return McpToolResult.CreateTextResult($"No se encontraron datos en el rango '{Range}'");
//                 }

//                 // Formatear los datos para el LLM
//                 var formattedData = FormatDataForLLM(data);

//                 return McpToolResult.CreateTextResult(formattedData);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, $"Error al leer el rango {Range}");
//                 return McpToolResult.CreateError($"Error al leer los datos: {ex.Message}");
//             }
//         }
//     }
// }