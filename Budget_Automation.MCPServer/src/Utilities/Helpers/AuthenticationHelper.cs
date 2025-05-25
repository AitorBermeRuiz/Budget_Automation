using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;

namespace Budget_Automation.MCPServer.Utilities.Helpers
{
    public static class AuthenticationHelper
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        public static async Task RunAuthenticationFlow()
        {
            Console.WriteLine("=== Configuración de Autenticación Google Sheets ===\n");
            
            var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_PATH") 
                ?? "credentials/client_secret.json";
            var tokenPath = Environment.GetEnvironmentVariable("GOOGLE_TOKEN_PATH") 
                ?? "credentials/tokens";

            Console.WriteLine($" Buscando credenciales en: {Path.GetFullPath(credentialsPath)}");

            if (!File.Exists(credentialsPath))
            {
                Console.WriteLine("No se encontró el archivo de credenciales.");
                Console.WriteLine("Instrucciones:");
                Console.WriteLine("1. Ve a https://console.cloud.google.com");
                Console.WriteLine("2. Crea un proyecto y habilita Google Sheets API");
                Console.WriteLine("3. Crea credenciales OAuth 2.0 (tipo 'Desktop App')");
                Console.WriteLine("4. Descarga el JSON y guárdalo como:");
                Console.WriteLine($"   {Path.GetFullPath(credentialsPath)}");
                return;
            }

            try
            {
                Directory.CreateDirectory(tokenPath);

                using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
                
                Console.WriteLine("\n Abriendo navegador para autenticación...\n");

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true));

                Console.WriteLine("\n¡Autenticación completada!");
                Console.WriteLine($" Tokens guardados en: {Path.GetFullPath(tokenPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error: {ex.Message}");
            }
        }

        public static void VerifyTokensExist()
        {
            var tokenPath = Environment.GetEnvironmentVariable("GOOGLE_TOKEN_PATH") 
                ?? "credentials/tokens";
            var tokenFile = Path.Combine(tokenPath, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");

            if (!File.Exists(tokenFile))
            {
                Console.Error.WriteLine("No se encontraron tokens de autenticación.");
                Console.Error.WriteLine("\nEjecuta primero:");
                Console.Error.WriteLine("   - dotnet run auth");
                Environment.Exit(1);
            }
        }
    }
}