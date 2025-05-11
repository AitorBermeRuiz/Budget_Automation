using System;

namespace Budget_Automation.MCPServer.Services.Google.Abstract;

public interface IGoogleSheetsClient
{
    Task<IList<IList<object>>> ReadRangeAsync(string range);
}
