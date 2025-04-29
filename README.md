# Expense Tracker MCP Server

Un servidor MCP (Model Context Protocol) para integrar Claude LLM con Google Sheets, permitiendo rastrear y analizar gastos personales mediante lenguaje natural.

![Expense Tracker Banner](https://via.placeholder.com/800x200/e6f7ff/0366d6?text=Expense+Tracker+MCP+Server)

## 📋 Descripción

Este proyecto implementa un servidor MCP personalizado que actúa como intermediario entre Claude y Google Sheets para crear un sistema de rastreo de gastos completamente funcional. Permite:

- Registrar gastos e ingresos en Google Sheets mediante lenguaje natural
- Categorizar automáticamente transacciones
- Comparar gastos reales con presupuestos
- Generar informes y análisis financieros personalizados
- Procesar datos de emails con información de gastos (a través de un script de Gmail)

## 🚀 Características

- **Operaciones CRUD completas**: Añadir, consultar, actualizar y eliminar gastos
- **Análisis financiero**: Obtener informes sobre patrones de gasto y cumplimiento de presupuesto
- **Integración con Google Sheets**: Utiliza tu hoja de cálculo como base de datos
- **API REST**: Interfaz clara para integración con Claude y otros sistemas
- **Seguridad**: Autenticación mediante credenciales de servicio de Google Cloud
- **Configuración flexible**: Adaptable a diferentes estructuras de hojas de cálculo

## 🛠️ Tecnologías

- **C# (.NET 6+)**: Lenguaje y framework principal
- **Google Sheets API**: Para operaciones de lectura/escritura en hojas de cálculo
- **Newtonsoft.Json**: Para procesamiento de JSON
- **HttpListener**: Para el manejo de solicitudes HTTP

## 📦 Requisitos Previos

- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) o superior
- [Cuenta de Google Cloud](https://console.cloud.google.com/) con API de Google Sheets habilitada
- Un archivo de credenciales de servicio de Google Cloud
- Una hoja de cálculo de Google configurada para el rastreo de gastos

## ⚙️ Instalación

1. Clona este repositorio:
   ```bash
   git clone https://github.com/tuusuario/expense-tracker-mcp.git
   cd expense-tracker-mcp
   ```

2. Restaura los paquetes NuGet:
   ```bash
   dotnet restore
   ```

3. Coloca tu archivo de credenciales de Google Cloud (`credentials.json`) en la carpeta raíz del proyecto.

4. Actualiza el ID de tu hoja de cálculo en `Program.cs`:
   ```csharp
   private string _spreadsheetId = "TU_ID_DE_SPREADSHEET";
   ```

5. Compila el proyecto:
   ```bash
   dotnet build
   ```

## 🚦 Uso

### Iniciando el Servidor

```bash
dotnet run
```

El servidor comenzará a escuchar en `http://localhost:8080/`.

### Endpoints Disponibles

El servidor expone una API JSON a través de un único endpoint que acepta diferentes acciones:

| Acción | Descripción | Parámetros |
|--------|-------------|------------|
| `addExpense` | Añade un nuevo gasto a la hoja | `date`, `type`, `category`, `amount`, `notes` |
| `getExpenses` | Obtiene lista de gastos (con filtros opcionales) | `month`, `year`, `category` (todos opcionales) |
| `getBudget` | Obtiene información del presupuesto | - |
| `getMonthlyReport` | Genera un informe mensual | `month`, `year` |

### Ejemplo de Solicitud

```json
{
  "action": "addExpense",
  "date": "2025-04-29",
  "type": "Gasto",
  "category": "Comida",
  "amount": "15.50",
  "notes": "Restaurante con amigos"
}
```

### Ejemplo de Respuesta

```json
{
  "success": true,
  "message": "Gasto añadido correctamente",
  "data": {
    "date": "2025-04-29",
    "type": "Gasto",
    "category": "Comida",
    "amount": "15.50",
    "notes": "Restaurante con amigos"
  }
}
```

## 📋 Estructura del Proyecto

```
ExpenseTrackerMCP/
├── ExpenseTrackerMCP.csproj    # Archivo de proyecto .NET
├── Program.cs                  # Código principal del servidor
├── GoogleSheetsService.cs      # Clase auxiliar para Google Sheets
├── credentials.json            # Credenciales de Google Cloud (no incluido)
└── README.md                   # Este archivo
```

## 🔄 Integración con Google Sheets

Este servidor espera una estructura específica en Google Sheets:

### Hoja "Gastos"
Columnas esperadas (A-E):
- Fecha (A)
- Tipo (B) - "Ingreso" o "Gasto"
- Categoría (C)
- Importe (D)
- Notas (E)

### Hoja "Presupuesto"
Columnas esperadas (A-B):
- Categoría (A)
- Presupuesto Mensual (B)

## 🔗 Integración con Claude

Este servidor está diseñado para integrarse con Claude a través del protocolo MCP. Claude puede:

1. Interpretar solicitudes en lenguaje natural
2. Llamar al MCP Server con los datos estructurados
3. Recibir y analizar la respuesta
4. Presentar la información al usuario en formato conversacional

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del repositorio
2. Crea una rama para tu característica (`git checkout -b feature/nueva-caracteristica`)
3. Haz commit de tus cambios (`git commit -am 'Añade nueva característica'`)
4. Push a la rama (`git push origin feature/nueva-caracteristica`)
5. Crea un nuevo Pull Request

## 📃 Licencia

Este proyecto está licenciado bajo la Licencia MIT - consulta el archivo `LICENSE` para más detalles.

## 🙏 Agradecimientos

- [Google Sheets API](https://developers.google.com/sheets/api) por proporcionar una API robusta para hojas de cálculo
- [Model Context Protocol](https://github.com/modelcontextprotocol/mcp) por establecer el estándar para la comunicación con LLMs
- [Anthropic Claude](https://www.anthropic.com/claude) por proporcionar la inteligencia detrás del análisis

## 📞 Contacto

Si tienes preguntas o sugerencias, no dudes en:
- Abrir un issue en este repositorio
- Contactarme en [tu-email@example.com](mailto:aitorbermeruiz@gmail.com)

---

⭐️ Este proyecto es parte de un sistema personal de rastreo de gastos que integra Google Sheets, Claude LLM y automatización de correos electrónicos.
