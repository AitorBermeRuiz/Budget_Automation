using System;

namespace Budget_Automation.MCPServer.Services.Google.Models
{
    public class GoogleApiSettings
    {
        public string SpreadsheetId { get; set; } = string.Empty;
        public string TokenPath { get; set; } = "credentials/tokens";
        public string CredentialsPath { get; set; } = "credentials/client_secret.json";
        
    }
}