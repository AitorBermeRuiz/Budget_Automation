# Expense Tracking System with Google Sheets and Claude

## Project Overview
This project aims to develop a personal expense tracking system using Google Sheets for data storage and Claude as the natural language processing and analysis engine. The system will automate expense data entry and provide detailed reports on spending patterns and budget compliance.

## Core Components

### 1. Google Sheets Structure
- **Gastos (Expenses)** sheet for recording transactions
- **Presupuesto (Budget)** sheet for defining budget limits by category
- **Dashboard** sheet for visualization and reports

### 2. Google Apps Script
- Handles CRUD operations on Google Sheets
- Processes email notifications from banks automatically
- Exposes web endpoints for external communication

### 3. MCP Server (C#)
- Serves as intermediary between Claude and Google Sheets
- Processes natural language to extract financial data
- Communicates with Google Apps Script API

### 4. Claude Integration
- Provides natural language interface for users
- Interprets expense entries and queries
- Generates personalized financial reports

## Implementation Plan
1. Set up Google Sheets with appropriate structure and formulas
2. Develop Google Apps Script for data operations and email processing
3. Create MCP Server using C# and Streamable HTTP
4. Configure Claude to interface with the MCP Server

## MCP Server Architecture
- **Controllers**: API endpoints for MCP protocol
- **Services**: Business logic including NLP and Google integration
- **Models**: Data structures organized by service
- **Infrastructure**: DI, middleware, and configuration
- **Utilities**: Helpers and extensions

## Project Structure
```
./Budget_Automation/
├── Budget_Automation.sln
└── Budget_Automation.MCPServer/
    ├── src/
    │   ├── Program.cs
    │   ├── Budget_Automation.MCPServer.csproj
    │   ├── Controllers/
    │   ├── Services/
    │   │   ├── NaturalLanguage/
    │   │   │   ├── Models/
    │   │   ├── Google/
    │   │   │   ├── Models/
    │   │   └── Core/
    │   │       ├── Models/
    │   ├── MCP/
    │   │   ├── Models/
    │   ├── Infrastructure/
    │   └── Utilities/
    ├── test/
    ├── scripts/
    └── samples/
```
