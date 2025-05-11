using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;

namespace Budget_Automation.MCPServer.Services.Google.Abstract
{
        public interface IGoogleAuthService
    {
        Task<SheetsService> GetSheetsServiceAsync();
        Task<UserCredential> GetUserCredentialAsync();
    }
}