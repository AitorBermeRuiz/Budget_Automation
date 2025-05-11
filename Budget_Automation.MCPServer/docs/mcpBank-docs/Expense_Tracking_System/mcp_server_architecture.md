# MCP Server Architecture

## Project Structure
```
./Budget_Automation/
├── Budget_Automation.sln
└── Budget_Automation.MCPServer/
    ├── src/
    │   ├── Program.cs
    │   ├── Budget_Automation.MCPServer.csproj
    │   ├── Controllers/
    │   │   ├── MCPController.cs
    │   │   └── HealthCheckController.cs
    │   ├── Services/
    │   │   ├── NaturalLanguage/
    │   │   │   ├── Models/
    │   │   │   │   ├── ExpenseParseResult.cs
    │   │   │   │   ├── CategoryMatch.cs
    │   │   │   │   └── DateTimeExtraction.cs
    │   │   │   ├── ExpenseParser.cs
    │   │   │   ├── CategoryClassifier.cs
    │   │   │   └── DateRecognizer.cs
    │   │   ├── Google/
    │   │   │   ├── Models/
    │   │   │   │   ├── GoogleSheetsRequest.cs
    │   │   │   │   ├── GoogleSheetsResponse.cs
    │   │   │   │   ├── AddGastoRequest.cs
    │   │   │   │   ├── GetDashboardRequest.cs
    │   │   │   │   └── GastosByMonthRequest.cs
    │   │   │   ├── GoogleSheetsClient.cs
    │   │   │   ├── GoogleAuthService.cs
    │   │   │   └── AppScriptService.cs
    │   │   └── Core/
    │   │       ├── Models/
    │   │       │   ├── Expense.cs
    │   │       │   ├── Budget.cs
    │   │       │   ├── Category.cs
    │   │       │   └── Dashboard.cs
    │   │       ├── BudgetAnalyzer.cs
    │   │       ├── ExpenseManager.cs
    │   │       └── ReportGenerator.cs
    │   ├── MCP/
    │   │   ├── Models/
    │   │   │   ├── MCPRequest.cs
    │   │   │   ├── MCPResponse.cs
    │   │   │   └── MCPMetadata.cs
    │   │   ├── MCPRequestProcessor.cs
    │   │   └── MCPResponseFormatter.cs
    │   ├── Infrastructure/
    │   │   ├── DependencyInjection/
    │   │   │   └── ServiceCollectionExtensions.cs
    │   │   ├── Middleware/
    │   │   │   ├── ErrorHandlingMiddleware.cs
    │   │   │   └── RequestLoggingMiddleware.cs
    │   │   └── Configuration/
    │   │       ├── GoogleApiOptions.cs
    │   │       └── MCPOptions.cs
    │   └── Utilities/
    │       ├── Extensions/
    │       │   ├── StringExtensions.cs
    │       │   └── DateTimeExtensions.cs
    │       ├── Helpers/
    │       │   ├── RegexHelper.cs
    │       │   └── CurrencyHelper.cs
    │       └── Constants/
    │           └── RegexPatterns.cs
    ├── test/
    │   ├── Budget_Automation.MCPServer.Tests.csproj
    │   ├── Unit/
    │   │   ├── NaturalLanguage/
    │   │   │   ├── ExpenseParserTests.cs
    │   │   │   └── CategoryClassifierTests.cs
    │   │   └── MCP/
    │   │       └── MCPRequestProcessorTests.cs
    │   └── Integration/
    │       ├── GoogleServices/
    │       │   └── GoogleSheetsClientTests.cs
    │       └── EndToEnd/
    │           └── MCPControllerTests.cs
    ├── scripts/
    │   ├── build.sh
    │   └── deploy.sh
    └── samples/
        ├── expense_requests.json
        └── dashboard_requests.json
```

## Key Components

### Controllers
- **MCPController**: Processes incoming MCP requests from Claude
- **HealthCheckController**: Provides endpoints for monitoring system health

### Services
- **NaturalLanguage**: Parses user text to extract expense data
- **Google**: Communicates with Google Apps Script web endpoints
- **Core**: Handles business logic for expense and budget management

### MCP Layer
- **MCPRequestProcessor**: Processes incoming MCP requests
- **MCPResponseFormatter**: Formats responses according to MCP protocol

### Infrastructure
- **DependencyInjection**: Configures service registration
- **Middleware**: Handles cross-cutting concerns like logging and error handling
- **Configuration**: Manages application settings

### Utilities
- **Extensions**: Extension methods for common operations
- **Helpers**: Utility classes for specific functions
- **Constants**: Application-wide constants

## Core Implementation Elements

### Program.cs
```csharp
using Budget_Automation.MCPServer.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddControllers();
builder.Services.AddMCPServices(builder.Configuration);
builder.Services.AddGoogleServices(builder.Configuration);
builder.Services.AddNaturalLanguageServices();
builder.Services.AddCoreServices();

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
```

### MCPController.cs
```csharp
using Budget_Automation.MCPServer.MCP;
using Budget_Automation.MCPServer.MCP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Budget_Automation.MCPServer.Controllers
{
    [ApiController]
    [Route("api/mcp")]
    public class MCPController : ControllerBase
    {
        private readonly IMCPRequestProcessor _requestProcessor;

        public MCPController(IMCPRequestProcessor requestProcessor)
        {
            _requestProcessor = requestProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRequest([FromBody] MCPRequest request)
        {
            var response = await _requestProcessor.ProcessAsync(request);
            return Ok(response);
        }

        [HttpGet("metadata")]
        public IActionResult GetMetadata()
        {
            return Ok(new MCPMetadata
            {
                Name = "Budget Automation MCP Server",
                Description = "MCP Server for financial tracking with Google Sheets",
                Version = "1.0.0"
            });
        }
    }
}
```

### MCPRequestProcessor.cs
```csharp
using Budget_Automation.MCPServer.MCP.Models;
using Budget_Automation.MCPServer.Services.NaturalLanguage;
using Budget_Automation.MCPServer.Services.Core;
using System.Threading.Tasks;

namespace Budget_Automation.MCPServer.MCP
{
    public interface IMCPRequestProcessor
    {
        Task<MCPResponse> ProcessAsync(MCPRequest request);
    }

    public class MCPRequestProcessor : IMCPRequestProcessor
    {
        private readonly IExpenseParser _expenseParser;
        private readonly ICategoryClassifier _categoryClassifier;
        private readonly IExpenseManager _expenseManager;
        private readonly IMCPResponseFormatter _responseFormatter;

        public MCPRequestProcessor(
            IExpenseParser expenseParser,
            ICategoryClassifier categoryClassifier,
            IExpenseManager expenseManager,
            IMCPResponseFormatter responseFormatter)
        {
            _expenseParser = expenseParser;
            _categoryClassifier = categoryClassifier;
            _expenseManager = expenseManager;
            _responseFormatter = responseFormatter;
        }

        public async Task<MCPResponse> ProcessAsync(MCPRequest request)
        {
            // 1. Procesar el texto en lenguaje natural
            var parseResult = _expenseParser.Parse(request.Query);
            
            // 2. Si es un gasto, procesarlo
            if (parseResult.IsExpense)
            {
                var category = _categoryClassifier.Classify(parseResult.Description);
                var result = await _expenseManager.AddExpenseAsync(
                    parseResult.Amount,
                    category,
                    parseResult.Date,
                    parseResult.Description);
                
                return _responseFormatter.FormatAddExpenseResponse(result);
            }
            
            // 3. Si es una consulta, procesarla
            if (parseResult.IsQuery)
            {
                var result = await _expenseManager.GetDashboardAsync(
                    parseResult.Month,
                    parseResult.Year);
                
                return _responseFormatter.FormatDashboardResponse(result);
            }
            
            // 4. Si no se reconoce, devolver error
            return new MCPResponse
            {
                Success = false,
                Message = "No pude entender la solicitud. Prueba con un formato como 'Gasté 15€ en comida' o 'Muéstrame el resumen del mes'."
            };
        }
    }
}
```

### ExpenseParser.cs (Partial Implementation)
```csharp
using Budget_Automation.MCPServer.Services.NaturalLanguage.Models;
using Budget_Automation.MCPServer.Utilities.Constants;
using Budget_Automation.MCPServer.Utilities.Helpers;
using System;
using System.Text.RegularExpressions;

namespace Budget_Automation.MCPServer.Services.NaturalLanguage
{
    public interface IExpenseParser
    {
        ExpenseParseResult Parse(string query);
    }

    public class RegexExpenseParser : IExpenseParser
    {
        public ExpenseParseResult Parse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return ExpenseParseResult.NotRecognized();
            }

            // Convertir a minúsculas para facilitar el procesamiento
            var processedQuery = query.ToLower();

            // Intentar identificar un gasto
            var expenseMatch = Regex.Match(
                processedQuery,
                RegexPatterns.ExpensePattern,
                RegexOptions.IgnoreCase);

            if (expenseMatch.Success)
            {
                return ParseExpense(expenseMatch);
            }

            // Intentar identificar una consulta
            var queryMatch = Regex.Match(
                processedQuery, 
                RegexPatterns.QueryPattern,
                RegexOptions.IgnoreCase);

            if (queryMatch.Success)
            {
                return ParseQuery(queryMatch);
            }

            // No se reconoció ningún patrón
            return ExpenseParseResult.NotRecognized();
        }
        
        // Métodos para parsear gastos y consultas se implementarían aquí
    }
}
```

### Model Classes

#### MCPRequest.cs
```csharp
namespace Budget_Automation.MCPServer.MCP.Models
{
    public class MCPRequest
    {
        public string Query { get; set; }
    }
}
```

#### MCPResponse.cs
```csharp
namespace Budget_Automation.MCPServer.MCP.Models
{
    public class MCPResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
```

#### Expense.cs
```csharp
using System;

namespace Budget_Automation.MCPServer.Services.Core.Models
{
    public class Expense
    {
        public DateTime Date { get; set; }
        public Category Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
```

#### Category.cs
```csharp
namespace Budget_Automation.MCPServer.Services.Core.Models
{
    public enum Category
    {
        Food,
        Clothing,
        Car,
        Investment,
        Other
    }
}
```

## Implementation Notes

### RegexPatterns.cs
```csharp
namespace Budget_Automation.MCPServer.Utilities.Constants
{
    public static class RegexPatterns
    {
        // Patrón para identificar gastos en lenguaje natural
        public const string ExpensePattern = @"(?:gasto|gastado|pagado|comprado|compra)(?:\s+de)?\s+(?<amount>\d+(?:[.,]\d+)?)\s*(?:€|euros?)(?:\s+en\s+(?<description>.+?)(?:\s+(?:ayer|hoy|el|anteayer|el día|el dia|hace|el pasado|la semana pasada))?)?\s*(?:el|en el|la|en la|del|ayer|hoy|anteayer)?\s*(?:día|dia)?\s*(?:(?<day>\d{1,2})[/-](?<month>\d{1,2})(?:[/-](?<year>\d{2,4}))?)?";

        // Patrón para identificar consultas de gastos
        public const string QueryPattern = @"(?:cuánto|cuanto|que|qué)(?:\s+he)?\s+(?:gastado|gasté|gaste)(?:\s+en)?\s+(?:el mes|este mes|el mes actual|el mes pasado|el mes de (?<month>\w+))?(?:\s+de)?\s*(?:(?<year>\d{4}))?";

        // Patrón para identificar solicitudes de dashboard
        public const string DashboardPattern = @"(?:resumen|dashboard|informe|estadísticas|estadisticas|resumen)(?:\s+de)?\s+(?:gastos|finanzas|gasto)?(?:\s+(?:del|de|este))?(?:\s+(?:mes|mes actual|mes pasado|mes de (?<month>\w+)))?(?:\s+(?:del|de|este))?(?:\s+(?:año|ano|año actual|año pasado))?(?:\s+(?<year>\d{4}))?";
    }
}
```

### ExpenseParseResult.cs
```csharp
using System;

namespace Budget_Automation.MCPServer.Services.NaturalLanguage.Models
{
    public class ExpenseParseResult
    {
        public bool IsRecognized { get; private set; }
        public bool IsExpense { get; private set; }
        public bool IsQuery { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public string Month { get; private set; }
        public int Year { get; private set; }

        // Factory methods
        public static ExpenseParseResult NotRecognized()
        {
            return new ExpenseParseResult
            {
                IsRecognized = false
            };
        }

        public static ExpenseParseResult Expense(decimal amount, string description, DateTime date)
        {
            return new ExpenseParseResult
            {
                IsRecognized = true,
                IsExpense = true,
                Amount = amount,
                Description = description,
                Date = date
            };
        }

        public static ExpenseParseResult Query(string month, int year)
        {
            return new ExpenseParseResult
            {
                IsRecognized = true,
                IsQuery = true,
                Month = month,
                Year = year
            };
        }
    }
}
```

## Unit Testing Example

### ExpenseParserTests.cs
```csharp
using Budget_Automation.MCPServer.Services.NaturalLanguage;
using Budget_Automation.MCPServer.Services.NaturalLanguage.Models;
using Xunit;

namespace Budget_Automation.MCPServer.Tests.Unit.NaturalLanguage
{
    public class ExpenseParserTests
    {
        private readonly IExpenseParser _parser;

        public ExpenseParserTests()
        {
            _parser = new RegexExpenseParser();
        }

        [Fact]
        public void Parse_ExpensePattern_ShouldReturnExpenseResult()
        {
            // Arrange
            var query = "He gastado 15€ en comida ayer";

            // Act
            var result = _parser.Parse(query);

            // Assert
            Assert.True(result.IsRecognized);
            Assert.True(result.IsExpense);
            Assert.Equal(15, result.Amount);
            Assert.Contains("comida", result.Description);
        }

        [Fact]
        public void Parse_QueryPattern_ShouldReturnQueryResult()
        {
            // Arrange
            var query = "¿Cuánto he gastado este mes?";

            // Act
            var result = _parser.Parse(query);

            // Assert
            Assert.True(result.IsRecognized);
            Assert.True(result.IsQuery);
            Assert.NotNull(result.Month);
        }

        [Fact]
        public void Parse_InvalidInput_ShouldReturnNotRecognized()
        {
            // Arrange
            var query = "Hola, ¿cómo estás?";

            // Act
            var result = _parser.Parse(query);

            // Assert
            Assert.False(result.IsRecognized);
        }
    }
}
```

## Key Technical Considerations

1. **Natural Language Processing**: El componente central del sistema es la capacidad de interpretar solicitudes en lenguaje natural para agregar gastos o consultar información.

2. **Patrones Regex**: Se utilizan expresiones regulares cuidadosamente diseñadas para extraer información financiera de texto no estructurado.

3. **Integración con Google Sheets**: El servidor se comunica con Google Apps Script para operaciones CRUD en las hojas de cálculo.

4. **Arquitectura Limpia**: Separación clara de responsabilidades en capas (controladores, servicios, modelos).

5. **Soporte MCP**: Implementación completa del protocolo MCP para integrarse con Claude.

6. **Manejo de Errores**: Sistema robusto de gestión de errores y validación de entradas.

7. **Testing**: Enfoque en pruebas unitarias y de integración para garantizar la calidad del código.
