using System.Threading.Tasks;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Budget_Automation.MCPServer.Services.Google.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Budget_Automation.MCPServer.Services.Google
{

    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly GoogleAuthService _authService;
        private readonly GoogleApiSettings _settings;
        private readonly ILogger<GoogleSheetsService> _logger;
        private SheetsService? _sheetsService;

        public GoogleSheetsService(
            GoogleAuthService authService,
            GoogleApiSettings settings,   
            ILogger<GoogleSheetsService> logger)
        {
            _authService = authService;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Reads the specified ranges from a Google Sheets spreadsheet and returns the batch of values.
        /// </summary>
        public async Task<BatchGetValuesResponse> ReadRange(List<string> ranges) 
        {
            try 
            {
                _logger.LogDebug($"Leyendo rangos: {string.Join(", ", ranges)} de {_settings.SpreadsheetId}");
                
                // Obtener credenciales y crear servicio
                var credential = await _authService.LoginAsync();
                
                if (_sheetsService == null)
                {
                    _sheetsService = new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Budget_Automation"
                    });
                }

                var request = _sheetsService.Spreadsheets.Values.BatchGet(_settings.SpreadsheetId); 
                request.Ranges = ranges;
                
                var response = await request.ExecuteAsync();
                
                _logger.LogInformation($"Lectura exitosa. Rangos devueltos: {response.ValueRanges?.Count ?? 0}");
                
                return response;

            } 
            catch (global::Google.GoogleApiException gEx)
            {
                _logger.LogError($"Error de Google API: {gEx.Message}, Status: {gEx.HttpStatusCode}");
                throw new Exception($"Error al leer los rangos {string.Join(", ", ranges)}: {gEx.Message}", gEx);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Error al leer los rangos {string.Join(", ", ranges)}");
                throw new Exception($"Error al leer los rangos {string.Join(", ", ranges)}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un rango de celdas en una hoja de Google Sheets
        /// </summary>
        public async Task<UpdateValuesResponse> UpdateRange(string range, IList<IList<object>> values)
        {
            try
            {
                _logger.LogDebug($"Escribiendo en rango: {range} de {_settings.SpreadsheetId}");
                
                // Obtener credenciales y crear servicio si no existe
                var credential = await _authService.LoginAsync();
                
                if (_sheetsService == null)
                {
                    _sheetsService = new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Budget_Automation"
                    });
                }

                // Crear el objeto ValueRange con los nuevos valores
                var valueRange = new ValueRange
                {
                    Values = values
                };

                // Crear la solicitud de actualización
                var updateRequest = _sheetsService.Spreadsheets.Values.Update(
                    valueRange, 
                    _settings.SpreadsheetId, 
                    range);
                
                // Especificar que los valores se deben interpretar como valores de usuario
                updateRequest.ValueInputOption = 
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                // Ejecutar la solicitud
                var googleResponse = await updateRequest.ExecuteAsync() 
                    ?? throw new Exception("No se recibió respuesta del servidor de Google Sheets");

                _logger.LogInformation($"Escritura exitosa. Celdas actualizadas: {googleResponse.UpdatedCells}");

                // Devolver respuesta
                return new UpdateValuesResponse
                {
                    SpreadsheetId = googleResponse.SpreadsheetId,
                    UpdatedRange = googleResponse.UpdatedRange,
                    UpdatedRows = googleResponse.UpdatedRows,
                    UpdatedColumns = googleResponse.UpdatedColumns,
                    UpdatedCells = googleResponse.UpdatedCells,
                    ETag = googleResponse.ETag
                };
            }
            catch (global::Google.GoogleApiException gEx)
            {
                _logger.LogError($"Error de Google API: {gEx.Message}, Status: {gEx.HttpStatusCode}");
                throw new Exception($"Error al actualizar el rango {range}: {gEx.Message}", gEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el rango {range}");
                throw new Exception($"Error al actualizar el rango {range}: {ex.Message}", ex);
            }
        }
    }
}
