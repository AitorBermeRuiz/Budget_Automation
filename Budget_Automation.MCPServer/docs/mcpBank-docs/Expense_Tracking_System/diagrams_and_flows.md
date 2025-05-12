# Diagramas del Sistema

## Diagrama de Arquitectura
![Diagrama de Arquitectura](https://placeholder-for-diagrams.com/architecture.png)

Este diagrama muestra la arquitectura general del sistema, incluyendo:
- Usuario que interactúa con Claude
- Claude que procesa lenguaje natural
- MCP Server que actúa como intermediario
- Google Apps Script para operaciones en hojas de cálculo
- Google Sheets como base de datos
- Gmail para automatización de correos

## Diagrama de Flujo de Datos
![Diagrama de Flujo de Datos](https://placeholder-for-diagrams.com/data-flow.png)

Este diagrama detalla cómo fluyen los datos a través del sistema:
1. **Entrada manual**: El usuario describe un gasto a Claude, que lo interpreta y envía al MCP Server
2. **Entrada automática**: Los correos bancarios se etiquetan en Gmail y son procesados automáticamente
3. **Consultas**: El usuario solicita informes o resúmenes que se generan desde los datos almacenados

## Diagrama de Modelo de Datos
![Diagrama de Modelo de Datos](https://placeholder-for-diagrams.com/data-model.png)

Este diagrama muestra la estructura de datos en Google Sheets:
- Hoja de **Gastos** con transacciones individuales
- Hoja de **Presupuesto** con límites por categoría
- Hoja de **Dashboard** con fórmulas para resúmenes y visualizaciones

## Principales Flujos de Usuarios

### Flujo 1: Registro Manual de Gastos
1. Usuario dice a Claude: "Gasté 15€ en comida ayer"
2. Claude invoca el MCP Server
3. MCP Server interpreta la frase, extrae importe (15€), categoría (comida) y fecha (ayer)
4. MCP Server envía datos a Google Apps Script
5. Google Apps Script registra el gasto en la hoja de cálculo
6. La respuesta recorre el camino inverso hasta el usuario

### Flujo 2: Automatización de Correos
1. El banco envía un correo electrónico de confirmación de compra
2. Gmail aplica la etiqueta "gastos" según filtros configurados
3. Google Apps Script procesa periódicamente los correos con esta etiqueta
4. El script extrae el importe y otros detalles
5. El gasto se registra automáticamente en Google Sheets

### Flujo 3: Consulta de Informes
1. Usuario pregunta: "¿Cuánto he gastado en comida este mes?"
2. Claude invoca el MCP Server
3. MCP Server interpreta la consulta
4. MCP Server solicita datos a Google Apps Script
5. Google Apps Script consulta la hoja de cálculo
6. MCP Server recibe los datos y los formatea
7. Claude presenta un informe detallado al usuario

## Notas Técnicas

### Patrones de Reconocimiento
El sistema utiliza expresiones regulares para identificar:
- Gastos: "Gasté X€ en Y"
- Consultas: "¿Cuánto he gastado en X?"
- Solicitudes de informes: "Dame un resumen del mes"

### Categorización Automática
El sistema categoriza gastos basándose en:
- Palabras clave en la descripción
- Patrones históricos de gastos similares
- Reglas predefinidas por categoría

### Procesamiento de Correos
La extracción de información de correos se basa en:
- Patrones recurrentes en el asunto
- Estructuras comunes en el cuerpo del mensaje
- Reconocimiento de emisores (bancos, servicios)
