using System;
using Google.Apis.Sheets.v4.Data;

namespace Budget_Automation.MCPServer.Services.Google.Abstract;

public interface IGoogleSheetsService
{
    Task<BatchGetValuesResponse> ReadRange(List<String> ranges);
    Task<BatchGetValuesResponse> GetRangeFormulas(List<String> ranges);
    Task<UpdateValuesResponse> UpdateRange(string range, IList<IList<Object>> values);
}
