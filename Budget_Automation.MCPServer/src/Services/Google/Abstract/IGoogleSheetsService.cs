using System;
using Google.Apis.Sheets.v4.Data;

namespace Budget_Automation.MCPServer.Services.Google.Abstract;

public interface IGoogleSheetsService
{
    BatchGetValuesResponse ReadRange(List<String> ranges);
    UpdateValuesResponse UpdateRange(string range, IList<IList<Object>> values);
}
