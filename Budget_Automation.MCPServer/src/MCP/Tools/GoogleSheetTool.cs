using System;
using System.ComponentModel;
using System.Text.Json;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Budget_Automation.MCPServer.MCP.Tools;

[McpServerToolType]
public class GoogleSheetTool(
    IGoogleSheetsService googleSheetsService,
    ILogger<GoogleSheetTool> logger)
{
    private readonly IGoogleSheetsService _googleSheetsService = googleSheetsService;
    private readonly ILogger<GoogleSheetTool> _logger = logger;

    [McpServerTool(Name = "get_ranges"), Description("Retrieve data from one or more specific ranges in the Google Sheets spreadsheet.")]
    public async Task<string> ReadRange(
        // [Description("A list of ranges from the Google Sheets spreadsheet to read. Each range should be specified in A1 notation. Example: [\"Presupuesto!A1:B10\", \"Dashboard!A1:D10\"]")] List<string> range)
        [Description("A list of ranges from the Google Sheets spreadsheet to read. Each range should be specified in A1 notation.")] List<string> range)
    {
        try
        {

            // Leer los datos del rango especificado
            var data =  await _googleSheetsService.ReadRange(range);

            // Si no hay datos, informar
            if (data == null || data.ValueRanges.Count == 0)
            {
                return $"No se encontraron datos en el rango '{range}'";
            }

            return JsonSerializer.Serialize(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al leer el rango {range}");
            return $"Error al leer los datos: {ex.Message}";
        }
    }

    [McpServerTool(Name = "write_range"), Description("Write data to a specific range in the Google Sheets spreadsheet.")]
    public async Task<string> WriteRange(
        [Description("The range in the Google Sheets spreadsheet where the data will be written. Example: Gastos!A4:E5")] string range,
        // [Description("The data to write in the specified range. Example: [[\"22/12/2024\", \"Ingreso\", \"Ocio\", 12, \"prueba 1\"], [\"22/12/2025\", \"Gasto\", \"Transporte\", 200, \"prueba 2\"]]")] List<IList<object>> values)
        [Description("The data to write in the specified range.")] List<IList<object>> values)
    {
    try
    {
        // Convertimos DateTimes a string con el formato dd/MM/yyyy
        var sanitizedValues = values.Select(row => row.Select(value => 
            value is DateTime dateTime ? dateTime.ToString("dd/MM/yyyy") : value?.ToString() ?? "").ToList()).ToList();
        // mapeamos a IList<object>
        var convertedValues = sanitizedValues
            .Select(row => row.Cast<object>().ToList() as IList<object>)
            .ToList();
        _logger.LogInformation("Attempting to write sanitized data to range {Range}: {Values}", range, JsonSerializer.Serialize(sanitizedValues));

        var response = await _googleSheetsService.UpdateRange(range, convertedValues);

        return $"Data successfully written to range '{response.UpdatedRange}'. Rows updated: {response.UpdatedRows}, Columns updated: {response.UpdatedColumns}.";
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error writing to range {Range} with values {Values}", range, JsonSerializer.Serialize(values));
        return $"Error writing data: {ex.Message}";
    }
    }
}
