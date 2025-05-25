using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Budget_Automation.MCPServer.Services.Google.Abstract;
using Budget_Automation.MCPServer.Services.Google.Models;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

namespace Budget_Automation.MCPServer.Services.Google
{
    public class GoogleAuthService
    {
        private readonly string[] scopes = [SheetsService.Scope.Spreadsheets];
        private readonly GoogleApiSettings _settings;
        private readonly ILogger<GoogleAuthService> _logger;
        private UserCredential? _credential;

        public GoogleAuthService(GoogleApiSettings settings, ILogger<GoogleAuthService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Login to Google API using OAuth2 with stored tokens.
        /// </summary>
        /// <returns>UserCredential for authentication</returns>
        public async Task<UserCredential> LoginAsync()
        {
            if (_credential != null)
            {
                return _credential;
            }

            try
            {
                _logger.LogDebug("Cargando credenciales desde {TokenPath}", _settings.TokenPath);

                // Cargar el token guardado
                var tokenStore = new FileDataStore(_settings.TokenPath, true);
                var token = await tokenStore.GetAsync<TokenResponse>("user");

                if (token == null)
                {
                    throw new InvalidOperationException(
                        "No se encontraron tokens. Ejecuta 'dotnet run auth' primero.");
                }

                // IMPORTANTE: Cargar los client secrets desde el archivo
                var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_PATH")
                    ?? _settings.CredentialsPath;

                ClientSecrets clientSecrets;
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    var googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);
                    clientSecrets = googleClientSecrets.Secrets;
                }

                // Crear el flujo con los client secrets reales
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = scopes,
                    DataStore = tokenStore
                });

                _credential = new UserCredential(flow, "user", token);

                // Intentar refrescar si es necesario
                if (token.IsStale)
                {
                    _logger.LogDebug("Token expirado, refrescando...");
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                }

                _logger.LogInformation("Autenticación cargada exitosamente");
                return _credential;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar credenciales");
                throw new InvalidOperationException(
                    "Error al cargar credenciales. Asegúrate de haber ejecutado 'dotnet run auth' primero.", ex);
            }
        }

    }
}