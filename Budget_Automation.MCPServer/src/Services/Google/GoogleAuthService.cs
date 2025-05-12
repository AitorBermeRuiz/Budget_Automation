
// using Google.Apis.Auth.OAuth2;
// using Google.Apis.Services;
// using Google.Apis.Sheets.v4;
// using Google.Apis.Util.Store;
// using Budget_Automation.MCPServer.Services.Google.Abstract;

// namespace Budget_Automation.MCPServer.Services.Google
// {
//     public class GoogleAuthService : IGoogleAuthService
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<GoogleAuthService> _logger;
//         private SheetsService? _sheetsService;
//         private UserCredential? _credential;

//         public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
//         {
//             _configuration = configuration;
//             _logger = logger;
//         }

//         public async Task<SheetsService> GetSheetsServiceAsync()
//         {
//             if (_sheetsService != null)
//             {
//                 return _sheetsService;
//             }

//             var credential = await GetUserCredentialAsync();

//             _sheetsService = new SheetsService(new BaseClientService.Initializer
//             {
//                 HttpClientInitializer = credential,
//                 ApplicationName = _configuration["GoogleApi:ApplicationName"] ?? 
//                     throw new InvalidOperationException("GoogleApi:ApplicationName configuration is missing")
//             });

//             return _sheetsService;
//         }

//         public async Task<UserCredential> GetUserCredentialAsync()
//         {
//             if (_credential != null)
//             {
//                 return _credential;
//             }

//             string? credentialsPath = _configuration["GoogleApi:CredentialsPath"];
            
//             if (string.IsNullOrEmpty(credentialsPath))
//             {
//                 throw new InvalidOperationException("GoogleApi:CredentialsPath configuration is missing");
//             }

//             if (!File.Exists(credentialsPath))
//             {
//                 _logger.LogError($"No se encontró el archivo de credenciales en: {credentialsPath}");
//                 throw new FileNotFoundException($"No se encontró el archivo de credenciales en: {credentialsPath}");
//             }

//             using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
//             {
//                 string? tokenFolderPath = _configuration["GoogleApi:TokenFolderPath"];
                
//                 if (string.IsNullOrEmpty(tokenFolderPath))
//                 {
//                     throw new InvalidOperationException("GoogleApi:TokenFolderPath configuration is missing");
//                 }

//                 // Crear el directorio de tokens si no existe
//                 if (!Directory.Exists(tokenFolderPath))
//                 {
//                     Directory.CreateDirectory(tokenFolderPath);
//                 }

//                 _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
//                     GoogleClientSecrets.FromStream(stream).Secrets,
//                     new[] { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive },
//                     "user",
//                     CancellationToken.None,
//                     new FileDataStore(tokenFolderPath, true)
//                 );

//                 _logger.LogInformation("Credencial guardada en: " + tokenFolderPath);
//                 return _credential;
//             }
//         }
//     }
// }