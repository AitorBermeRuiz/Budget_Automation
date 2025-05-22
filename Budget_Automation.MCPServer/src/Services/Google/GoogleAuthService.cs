using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Budget_Automation.MCPServer.Services.Google.Models;
using Microsoft.Extensions.Options;

namespace Budget_Automation.MCPServer.Services.Google
{
    public class GoogleAuthService
    {
        private readonly string googleClientId;
        private readonly string googleClientSecret;
        private readonly string[] scopes = [SheetsService.Scope.Spreadsheets];

        public GoogleAuthService(GoogleApiSettings settings)
        {
            googleClientId = settings.ClientId;
            googleClientSecret = settings.ClientSecret;
            scopes = [SheetsService.Scope.Spreadsheets];
        }

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

        // public GoogleCredential GetCredentialsFromFile()
        // {
        //     GoogleCredential credential;
        //     // Use Directory.GetCurrentDirectory() to resolve the credentials.json file path
        //     var filePath = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

        //     if (!File.Exists(filePath))
        //     {
        //         throw new FileNotFoundException($"The credentials file was not found at {filePath}");
        //     }

        //     using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //     {
        //         credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
        //     }
        //     return credential;
        // }
    }
}