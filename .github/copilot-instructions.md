# Copilot Instructions for Budget_Automation

## Project Overview
- **Architecture:**
  - Solution is organized as a .NET backend (`Budget_Automation.MCPServer`) with clear separation of concerns: Controllers, Infrastructure, Services, MCP Tools, and Utilities.
  - Data flows from Controllers → Services → Infrastructure, with external integrations handled in `Services/Google` and `MCP/Tools`.
  - Key configuration files: `appsettings.json`, `appsettings.Development.json`, and credentials in `src/credentials/`.

## Developer Workflows
- **Build:** Use the default build task or run `dotnet build Budget_Automation.MCPServer/src/Budget_Automation.MCPServer.csproj`.
- **Test:** Unit and integration tests are in `test/Unit` and `test/Integration`. Run with `dotnet test Budget_Automation.MCPServer/test/Budget_Automation.MCPServer.Tests.csproj`.
- **Debug:** Main entry point is `src/Program.cs`. Debugging typically starts here.

## Conventions & Patterns
- **Dependency Injection:** All services are registered via `Infrastructure/DependencyInjection`.
- **Configuration:** Use strongly-typed configuration via `Infrastructure/Configuration` and bind to `appsettings*.json`.
- **Google Integration:**
  - Service account credentials in `src/credentials/`.
  - Google Sheets logic in `Services/Google/GoogleSheetsService.cs` and tools in `MCP/Tools/GoogleSheetTool.cs`.
- **Tooling:** Custom tools are implemented in `MCP/Tools/` and follow a pattern of inheriting from a base tool class.
- **Constants & Helpers:** Shared logic is in `Utilities/Constants` and `Utilities/Helpers`.

## External Dependencies
- **Google APIs:** Used for Sheets and authentication. Credentials and tokens are managed in `src/credentials/`.
- **Weather API:** See `MCP/Tools/WeatherTool.cs` for integration details.

## Examples
- **Adding a new Google Sheet tool:**
  - Implement in `MCP/Tools/`, register in DI, and update configuration as needed.
- **Extending configuration:**
  - Add new config section in `appsettings.json`, create a POCO in `Infrastructure/Configuration`, and bind in DI.

## Key Files & Directories
- `src/Controllers/` – API endpoints
- `src/Infrastructure/` – DI, config, middleware
- `src/Services/Google/` – Google integration
- `src/MCP/Tools/` – Custom tools
- `src/Utilities/` – Shared helpers and constants
- `test/` – Unit and integration tests

## Special Notes
- **Sensitive Data:** Never commit credentials in `src/credentials/`.
- **Documentation:** See `docs/` and `docs/mcpBank-docs/Expense_Tracking_System/` for architecture and integration guides.

---
_Review and update this file as project structure or conventions evolve._
