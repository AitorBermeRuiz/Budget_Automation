# 1. Etapa de compilación (SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo del proyecto y restaurar las dependencias (Nuget)
# La ruta es relativa al contexto que definiremos en el docker-compose
COPY Budget_Automation.MCPServer/src/Budget_Automation.MCPServer.csproj ./Budget_Automation.MCPServer/src/
RUN dotnet restore Budget_Automation.MCPServer/src/Budget_Automation.MCPServer.csproj

# Copiar todo el código fuente y publicar la aplicación
COPY . .
RUN dotnet publish Budget_Automation.MCPServer/src/Budget_Automation.MCPServer.csproj -c Release -o /app/publish

# 2. Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto donde escucha el servidor MCP
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Comando para arrancar el servidor
ENTRYPOINT ["dotnet", "Budget_Automation.MCPServer.dll"]