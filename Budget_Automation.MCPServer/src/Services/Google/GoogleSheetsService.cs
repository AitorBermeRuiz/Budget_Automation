using Budget_Automation.MCPServer.Services.Google.Abstract;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Budget_Automation.MCPServer.Services.Google
{

    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly SheetsService sheetsService;
        private const string SpreadsheetId = "122ALcsCUMwzgveM-EZeha37dd6vdfxRiDO_1jVOfqdE";

        public GoogleSheetsService(GoogleAuthService googleAuthService)
        {
            var credentials = googleAuthService.LoginAsync().Result;
            sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials
            });
        }

        /// <summary>
        /// Reads the specified ranges from a Google Sheets spreadsheet and returns the batch of values.
        /// </summary>
        /// <param name="ranges">
        /// A list of A1 notation ranges to retrieve from the spreadsheet. 
        /// Each range should be specified as a string in the format "SheetName!A1:B2".
        /// <para>
        /// <b>Ejemplos de listas de rangos válidas:</b>
        /// <list type="bullet">
        ///   <item>
        ///     <description><c>new List&lt;string&gt; { "Sheet1!A1:C5", "Sheet2!D1:D10" }</c></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </param>
        /// <returns>
        /// Un objeto <see cref="BatchGetValuesResponse"/> que contiene los valores recuperados de los rangos especificados.
        /// </returns>
       public BatchGetValuesResponse ReadRange(List<String> ranges) 
        {
            try 
            {
                // TODO : Add the actual spreadsheet ID
                var request = sheetsService.Spreadsheets.Values.BatchGet(SpreadsheetId); 
                request.Ranges = ranges;
                return request.Execute();

            } catch (Exception ex) 
            {
                // Capturar cualquier excepción y lanzar una nueva con más contexto
                throw new Exception($"Error al leer los rangos {string.Join(", ", ranges)}: {ex.Message}", ex);
            }
        }

    /// <summary>
    /// Actualiza un rango de celdas en una hoja de Google Sheets
    /// </summary>
    /// <param name="spreadsheetId">ID de la hoja de cálculo de Google</param>
    /// <param name="range">Rango a actualizar (ej. "Sheet1!A1:D5")</param>
    /// <param name="values">Matriz de valores a escribir en el rango</param>
    /// <returns>Objeto con la respuesta de la actualización o lanza excepción si falla</returns>
    public UpdateValuesResponse UpdateRange(string range, IList<IList<Object>> values)
    {
        try
        {
            // Crear el objeto ValueRange con los nuevos valores
            var valueRange = new ValueRange
            {
                Values = values
            };

            // Crear la solicitud de actualización
                // TODO : Add the actual spreadsheet ID
            var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            
            // Especificar que los valores se deben interpretar como valores, no fórmulas
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            // Ejecutar la solicitud
            var googleResponse = updateRequest.Execute() ?? throw new Exception("No se recibió respuesta del servidor de Google Sheets");

                // Crear y devolver nuestro objeto de respuesta personalizado
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
        catch (Exception ex)
        {
            // Capturar cualquier excepción y lanzar una nueva con más contexto
            throw new Exception($"Error al actualizar el rango {string.Join(",", range)} en la hoja {SpreadsheetId}: {ex.Message}", ex);
        }
    }

        /// <summary>
        /// Creates a new instance of Google Sheets Service with the provided user credentials.
        /// </summary>
        /// <param name="credential">The user credential required for authentication with Google Sheets API.</param>
        /// <returns>A configured SheetsService instance ready to interact with Google Sheets.</returns>
        /// <remarks>
        /// This method initializes a new SheetsService with the specified user credentials for HTTP client authentication.
        /// </remarks>
        private SheetsService CreateService(UserCredential credential) =>
        
             new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential
                }
             );
    }
}
