// using Budget_Automation.MCPServer.Services.Google.Abstract;

// namespace Budget_Automation.MCPServer.Services.Google
// {

//     public class GoogleSheetsClient : IGoogleSheetsClient
//     {
//         private readonly IGoogleAuthService _authService;
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<GoogleSheetsClient> _logger;
//         private readonly string _spreadsheetId;

//         public GoogleSheetsClient(
//             IGoogleAuthService authService,
//             IConfiguration configuration,
//             ILogger<GoogleSheetsClient> logger)
//         {
//             _authService = authService;
//             _configuration = configuration;
//             _logger = logger;
//             _spreadsheetId = configuration["GoogleSheets:SpreadsheetId"] ??
//                 throw new InvalidOperationException("GoogleSheets:SpreadsheetId configuration is missing");
//         }

//         public async Task<IList<IList<object>>> ReadRangeAsync(string range)
//         {
//             try
//             {
//                 var service = await _authService.GetSheetsServiceAsync();
                
//                 // Realizar la solicitud para leer el rango especificado
//                 var request = service.Spreadsheets.Values.Get(_spreadsheetId, range);
//                 var response = await request.ExecuteAsync();
                
//                 if (response.Values == null || response.Values.Count == 0)
//                 {
//                     _logger.LogInformation($"No se encontraron datos en el rango {range}");
//                     return new List<IList<object>>();
//                 }
                
//                 _logger.LogInformation($"Se leyeron {response.Values.Count} filas del rango {range}");
//                 return response.Values;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError($"Error al leer el rango {range}: {ex.Message}");
//                 throw;
//             }
//         }
//     }
// }
