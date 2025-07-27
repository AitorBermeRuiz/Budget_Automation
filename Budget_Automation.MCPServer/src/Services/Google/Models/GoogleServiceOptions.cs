using System;
using System.ComponentModel.DataAnnotations;

namespace Budget_Automation.MCPServer.Services.Google.Models
{
    public class GoogleServiceOptions
    {        
        [Required]
        public string SpreadsheetId { get; set; } = string.Empty;
        
        [Required]
        public string ProjectId { get; set; } = string.Empty;
        
        [Required]
        public string PrivateKeyId { get; set; } = string.Empty;
        
        [Required]
        public string PrivateKey { get; set; } = string.Empty;
        
        [Required]
        public string ClientEmail { get; set; } = string.Empty;
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        public List<string> Scopes { get; set; } = new()
        {
            "https://www.googleapis.com/auth/spreadsheets",
            "https://www.googleapis.com/auth/drive.file"
        };
        
        public string AuthUri => "https://accounts.google.com/o/oauth2/auth";
        public string TokenUri => "https://oauth2.googleapis.com/token";
        public string AuthProviderX509CertUrl => "https://www.googleapis.com/oauth2/v1/certs";
        
        public string GetClientX509CertUrl()
        {
            return $"https://www.googleapis.com/robot/v1/metadata/x509/{Uri.EscapeDataString(ClientEmail)}";
        }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(SpreadsheetId) &&
                   !string.IsNullOrEmpty(ProjectId) &&
                   !string.IsNullOrEmpty(PrivateKeyId) &&
                   !string.IsNullOrEmpty(PrivateKey) &&
                   !string.IsNullOrEmpty(ClientEmail) &&
                   !string.IsNullOrEmpty(ClientId);
        }
        
        public string ToServiceAccountJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "service_account",
                project_id = ProjectId,
                private_key_id = PrivateKeyId,
                private_key = PrivateKey,
                client_email = ClientEmail,
                client_id = ClientId,
                auth_uri = AuthUri,
                token_uri = TokenUri,
                auth_provider_x509_cert_url = AuthProviderX509CertUrl,
                client_x509_cert_url = GetClientX509CertUrl()
            });
        }
    } 
}
