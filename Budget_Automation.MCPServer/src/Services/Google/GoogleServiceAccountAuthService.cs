using System;
using Budget_Automation.MCPServer.Services.Google.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Budget_Automation.MCPServer.Services.Google
{
    public class GoogleServiceAccountAuthService
    {
        private readonly GoogleServiceOptions _options;
        private readonly ILogger<GoogleServiceAccountAuthService> _logger;
        private GoogleCredential? _credential;

        public GoogleServiceAccountAuthService(
            IOptions<GoogleServiceOptions> options,
            ILogger<GoogleServiceAccountAuthService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public  GoogleCredential GetCredentialAsync()
        {
            if (_credential != null)
            {
                return _credential;
            }

            try
            {
                _logger.LogDebug("Creando credenciales desde configuración");

                if (!_options.IsValid())
                {
                    throw new InvalidOperationException(
                        "Configuración de GoogleService incompleta. " +
                        "Verifica la sección GoogleService en appsettings.json o variables de entorno.");
                }

                // Crear JSON dinámicamente desde la configuración
                var serviceAccountJson = _options.ToServiceAccountJson();
                
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serviceAccountJson));
                
                _credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(_options.Scopes);

                _logger.LogInformation("Credenciales de service account creadas exitosamente");
                _logger.LogDebug("Service Account Email: {Email}", _options.ClientEmail);
                _logger.LogDebug("Project ID: {ProjectId}", _options.ProjectId);
                
                return _credential;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando credenciales de service account");
                throw new InvalidOperationException(
                    "Error al crear credenciales de Google Service Account", ex);
            }
        }
    }   
}