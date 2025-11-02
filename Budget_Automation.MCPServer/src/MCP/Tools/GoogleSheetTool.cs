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
        [Description("A list of ranges from the Google Sheets spreadsheet to read. Each range should be specified in A1 notation.")] List<string> range)
    {
        try
        {
            // Leer los datos del rango especificado
            var data = await _googleSheetsService.ReadRange(range);
            
            // Si no hay datos, informar
            if (data == null || data.ValueRanges.Count == 0)
            {
                return $"No se encontraron datos en el rango '{string.Join(", ", range)}'";
            }
            
            return JsonSerializer.Serialize(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer el rango {Range}", string.Join(", ", range));
            return $"Error al leer los datos: {ex.Message}";
        }
    }

    [McpServerTool(Name = "get_range_formulas"), Description("Retrieve formulas from one or more specific ranges in the Google Sheets spreadsheet. Returns the actual formulas instead of calculated values.")]
    public async Task<string> GetRangeFormulas(
        [Description("A list of ranges from the Google Sheets spreadsheet to read formulas from. Each range should be specified in A1 notation.")] List<string> range)
    {
        try
        {
            // Leer las fórmulas del rango especificado
            var data = await _googleSheetsService.GetRangeFormulas(range);
            
            // Si no hay datos, informar
            if (data == null || data.ValueRanges.Count == 0)
            {
                return $"No se encontraron fórmulas en el rango '{string.Join(", ", range)}'";
            }
            
            return JsonSerializer.Serialize(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer las fórmulas del rango {Range}", string.Join(", ", range));
            return $"Error al leer las fórmulas: {ex.Message}";
        }
    }

    [McpServerTool(Name = "write_formulas"), Description("Write formulas to specific ranges in the Google Sheets spreadsheet and validate they work correctly. Automatically checks if formulas result in errors like #ERROR, #REF!, #DIV/0!, etc.")]
    public async Task<string> WriteFormulas(
        [Description("The range in the Google Sheets spreadsheet where the formulas will be written. Example: Presupuesto!C2:C10")] string range,
        [Description("The formulas to write. Each formula should start with '='. Example: [\"=SUM(A1:A10)\", \"=B2*C2\", \"=IF(D2>0,\"Positive\",\"Negative\")\"]")] List<string> formulas)
    {
        try
        {
            // Verificar que todas las fórmulas empiecen con '='
            var invalidFormulas = formulas.Where(f => !f.StartsWith("=")).ToList();
            if (invalidFormulas.Any())
            {
                return $"Error: Las siguientes fórmulas no empiezan con '=': {string.Join(", ", invalidFormulas)}";
            }

            // Convertir las fórmulas al formato requerido por UpdateRange
            var formulaValues = formulas.Select(formula => new List<object> { formula }.Cast<object>().ToList() as IList<object>).ToList();

            _logger.LogInformation("Attempting to write formulas to range {Range}: {Formulas}", range, JsonSerializer.Serialize(formulas));

            // 1. Escribir las fórmulas
            var writeResponse = await _googleSheetsService.UpdateRange(range, formulaValues);

            // 2. Esperaramos, google tiene que procesar las fórmulas.
            await Task.Delay(1000);

            // 3. Leer los valores calculados para verificar errores
            var readResponse = await _googleSheetsService.ReadRange(new List<string> { range });

            if (readResponse?.ValueRanges?.FirstOrDefault()?.Values != null)
            {
                var calculatedValues = readResponse.ValueRanges.First().Values;
                var errors = new List<string>();

                for (int i = 0; i < calculatedValues.Count; i++)
                {
                    if (calculatedValues[i]?.Count > 0)
                    {
                        var cellValue = calculatedValues[i][0]?.ToString() ?? "";
                        if (IsFormulaError(cellValue))
                        {
                            errors.Add($"Row {i + 1}: Formula '{formulas[i]}' resulted in error '{cellValue}'");
                        }
                    }
                }

                if (errors.Any())
                {
                    _logger.LogWarning("Formula errors detected: {Errors}", string.Join(", ", errors));
                    return $"❌ Formula errors detected:\n{string.Join("\n", errors)}\n\nPlease check the formulas and try again.";
                }
            }

            return $"✅ Formulas successfully written to range '{writeResponse.UpdatedRange}'. All formulas are working correctly. Rows updated: {writeResponse.UpdatedRows}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing formulas to range {Range} with formulas {Formulas}", range, JsonSerializer.Serialize(formulas));
            return $"Error writing formulas: {ex.Message}";
        }
    }

    private static bool IsFormulaError(string cellValue)
    {
        if (string.IsNullOrEmpty(cellValue))
            return false;

        // Detectar errores comunes de Google Sheets
        var errorPrefixes = new[]
        {
            "#ERROR!",
            "#DIV/0!",
            "#VALUE!",
            "#REF!",
            "#NAME?",
            "#NUM!",
            "#N/A",
            "#NULL!"
        };

        return errorPrefixes.Any(error => cellValue.StartsWith(error, StringComparison.OrdinalIgnoreCase));
    }

    [McpServerTool(Name = "write_range"), Description("Write data to a specific range in the Google Sheets spreadsheet.")]
    public async Task<string> WriteRange(
        [Description("The range in the Google Sheets spreadsheet where the data will be written. Example: Gastos!A4:E5")] string range,
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