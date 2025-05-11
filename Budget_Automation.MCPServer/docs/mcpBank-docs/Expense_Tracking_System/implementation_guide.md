# Guía de Implementación del Sistema de Gastos

## 1. Preparación de la Infraestructura

### Google Sheets
1. **Crear la hoja de cálculo en Google Drive**:
   - Crea una nueva hoja de cálculo de Google
   - Configura tres hojas: "Gastos", "Presupuesto" y "Dashboard"
   - Añade los encabezados y fórmulas según la estructura definida

2. **Copiar el ID de la hoja de cálculo**:
   - El ID se encuentra en la URL de la hoja: `https://docs.google.com/spreadsheets/d/[ID_DE_LA_HOJA]/edit`
   - Necesitarás este ID para el script de Google Apps Script

### Google Apps Script
1. **Crear un nuevo proyecto de Apps Script**:
   - En la hoja de cálculo, ve a Extensiones > Apps Script
   - Copia el código proporcionado en el editor
   - Actualiza la variable `SHEET_ID` con el ID de tu hoja

2. **Configurar permisos y desplegar como aplicación web**:
   - Ve a Implementar > Nueva implementación
   - Selecciona tipo "Aplicación web"
   - Configura:
     * Ejecutar como: "Tu cuenta"
     * Quién tiene acceso: "Cualquier persona" (para que el MCP Server pueda comunicarse)
   - Haz clic en "Implementar" y copia la URL generada
   - Autoriza los permisos solicitados

3. **Configurar el disparador para procesar correos**:
   - Ejecuta la función `configurarDisparador()` una vez
   - Esto establecerá el procesamiento automático cada hora

4. **Configuración de etiquetas en Gmail**:
   - En Gmail, crea una etiqueta llamada "gastos"
   - Opcionalmente, crea filtros para aplicar esta etiqueta automáticamente a correos de tu banco o servicios específicos

## 2. Desarrollo del MCP Server

### Configuración del entorno
1. **Instalar dependencias necesarias**:
   ```bash
   dotnet new web -n ExpenseTrackerMCP
   cd ExpenseTrackerMCP
   ```

2. **Agregar paquetes NuGet**:
   ```bash
   dotnet add package Microsoft.AspNetCore
   dotnet add package System.Text.Json
   ```

3. **Reemplazar el código del proyecto** con el código proporcionado para el MCP Server

4. **Actualizar la URL del script de Google**:
   - Modifica la variable `_googleScriptUrl` con la URL de la aplicación web de Google Apps Script

### Despliegue del MCP Server
1. **Publicar la aplicación**:
   ```bash
   dotnet publish -c Release
   ```

2. **Opciones de despliegue**:
   - **Local**: Ejecutar `dotnet run` y usar ngrok para exponer el puerto
   - **Servidor**: Desplegar en un VPS o servidor compartido con acceso público
   - **Azure/AWS**: Utilizar servicios en la nube (Azure App Service o AWS Elastic Beanstalk)

3. **Prueba de funcionamiento**:
   - Confirma que el endpoint `/api/mcp` responde correctamente
   - Realiza una prueba sencilla con Postman o curl

## 3. Integración con Claude

1. **Registrar el MCP Server con Claude**:
   - Seguir la documentación de Anthropic para registrar tu MCP Server
   - Proporcionar la URL de tu endpoint: `https://tu-servidor.com/api/mcp`
   - Configurar los metadatos del servidor para que aparezca con nombre descriptivo

2. **Ejemplos de prompts para Claude**:
   - "He gastado 15€ en comida ayer"
   - "¿Cuánto he gastado este mes?"
   - "Dame un resumen de mis finanzas"
   - "¿Cómo voy con el presupuesto de comida?"

## 4. Automatización de Correos con Gmail

### Configuración básica
1. **Etiquetado manual o automático**:
   - Etiquetar correos relevantes con la etiqueta "gastos"
   - Los correos con esta etiqueta serán procesados automáticamente

2. **Filtros automáticos**:
   - Crea filtros en Gmail basados en remitentes (banco, servicios)
   - Configura los filtros para aplicar la etiqueta "gastos" automáticamente

### Configuración avanzada (opcional)
1. **Mejora del procesamiento de correos**:
   - Amplía la función `procesarCorreosGastos()` en Google Apps Script
   - Añade patrones específicos para tus bancos o servicios
   - Implementa expresiones regulares más precisas para capturar importes y conceptos

2. **Procesamiento de archivos adjuntos**:
   - Para procesar PDFs o adjuntos, amplía el script para acceder a los adjuntos
   - Nota: Esto puede requerir APIs adicionales y será más complejo

## 5. Plan de Mantenimiento y Evolución

### Mantenimiento
1. **Copias de seguridad**:
   - Habilita el historial de versiones de Google Sheets
   - Programa una exportación mensual de los datos (CSV/Excel)

2. **Monitorización**:
   - Añade logs en Google Apps Script para revisar el funcionamiento
   - Configura notificaciones por email en caso de errores

### Evolución futura
1. **Mejoras a considerar**:
   - Implementar análisis predictivos de gastos futuros
   - Añadir visualizaciones más complejas (gráficos de tendencias)
   - Integración con más fuentes de datos (otras cuentas, inversiones)

2. **Escalabilidad**:
   - Si el volumen de datos crece, considera migrar a una base de datos más robusta
   - Mantén la interfaz con Google Sheets para la visualización

## 6. Resolución de Problemas Comunes

### Problemas de conexión
- **Timeout en Google Apps Script**: Las ejecuciones tienen un límite de tiempo. Evita operaciones muy largas.
- **Límites de cuota**: Google limita el número de ejecuciones diarias. Monitoriza el uso.

### Problemas de interpretación
- **Falsos positivos en correos**: Ajusta los patrones de reconocimiento para mejorar precisión.
- **Categorización incorrecta**: Refina las palabras clave utilizadas para cada categoría.

### Errores en el MCP Server
- **Problemas de CORS**: Configura correctamente los headers si Claude no puede acceder al servidor.
- **Manejo de excepciones**: Asegúrate de capturar y registrar todos los errores posibles.
