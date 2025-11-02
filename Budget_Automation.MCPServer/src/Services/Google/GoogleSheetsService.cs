using Budget_Automation.MCPServer.Services.Google.Abstract;
using Budget_Automation.MCPServer.Services.Google.Models;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Options;

namespace Budget_Automation.MCPServer.Services.Google
{

    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly GoogleServiceAccountAuthService _authService;
        private readonly GoogleServiceOptions _options;
        private readonly ILogger<GoogleSheetsService> _logger;
        private SheetsService? _sheetsService;

        public GoogleSheetsService(
            GoogleServiceAccountAuthService authService,
            IOptions<GoogleServiceOptions> options,
            ILogger<GoogleSheetsService> logger)
        {
            _authService = authService;
            _options = options.Value;
            _logger = logger;
        }



        public async Task<BatchGetValuesResponse> ReadRange(List<string> ranges)
            => await BatchGetWithOptions(ranges);

        public async Task<BatchGetValuesResponse> GetRangeFormulas(List<string> ranges)
            => await BatchGetWithOptions(ranges, SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.FORMULA);

        public async Task<UpdateValuesResponse> UpdateRange(string range, IList<IList<object>> values)
        {
            try
            {
                _logger.LogDebug("Escribiendo en rango: {Range} de spreadsheet {SpreadsheetId}",
                    range, _options.SpreadsheetId);

                var credential = _authService.GetCredential();

                if (_sheetsService == null)
                {
                    _sheetsService = new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Budget_Automation_MCP"
                    });
                }

                var valueRange = new ValueRange { Values = values };

                var updateRequest = _sheetsService.Spreadsheets.Values.Update(
                    valueRange,
                    _options.SpreadsheetId,
                    range);

                updateRequest.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                var googleResponse = await updateRequest.ExecuteAsync()
                    ?? throw new Exception("No se recibió respuesta del servidor de Google Sheets");

                _logger.LogInformation("Escritura exitosa. Celdas actualizadas: {Cells}",
                    googleResponse.UpdatedCells);

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
                _logger.LogError(ex, "Error al actualizar rango {Range}", range);
                throw;
            }
        }

        private async Task<BatchGetValuesResponse> BatchGetWithOptions(
            List<string> ranges,
            SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum? renderOption = null)
        {
            var credential = _authService.GetCredential();

            if (_sheetsService == null)
            {
                _sheetsService = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Budget_Automation_MCP"
                });
            }

            var request = _sheetsService.Spreadsheets.Values.BatchGet(_options.SpreadsheetId);
            request.Ranges = ranges;

            if (renderOption.HasValue)
                request.ValueRenderOption = renderOption.Value;

            return await request.ExecuteAsync();
        }
    }
}
