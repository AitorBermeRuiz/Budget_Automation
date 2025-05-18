
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Budget_Automation.MCPServer.Services.Google.Models;
using Microsoft.Extensions.Options;

namespace Budget_Automation.MCPServer.Services.Google
{
    public class GoogleAuthService(IOptions<GoogleApiSettings> settings)
    {
        private readonly string googleClientId = settings.Value.ClientId;
        private readonly string googleClientSecret = settings.Value.ClientSecret;
        private readonly string[] scopes = [SheetsService.Scope.Spreadsheets];

        /// <summary>
        /// Login to Google API using OAuth2.
        /// </summary>
        /// <param name="googleClientId"></param>
        /// <param name="googleClientSecret"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<UserCredential> LoginAsync()
        {
            ClientSecrets secrets = new()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, "user", CancellationToken.None);
        }
    }
}