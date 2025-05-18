using System;

namespace Budget_Automation.MCPServer.Services.Google.Models;

public class GoogleApiSettings
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string SpreadsheetId { get; set; }
}