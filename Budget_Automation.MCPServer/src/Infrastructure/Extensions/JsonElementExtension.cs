using System;
using System.Text.Json;

namespace Budget_Automation.MCPServer.Infrastructure.Extensions;

    public static class JsonElementExtensions
    {
        /// <summary>
        /// Determina si el JsonElement es nulo, indefinido o representa un objeto vacío
        /// </summary>
        /// <param name="element">El JsonElement a evaluar</param>
        /// <returns>True si es nulo o vacío, false en caso contrario</returns>
        public static bool IsNullOrEmpty(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null)
                return true;
                
            if (element.ValueKind == JsonValueKind.Undefined)
                return true;
                
            if (element.ValueKind == JsonValueKind.Object)
            {
                // Verificar si el objeto no tiene propiedades
                using (var enumerator = element.EnumerateObject())
                {
                    return !enumerator.MoveNext(); // Si no puede avanzar, está vacío
                }
            }
            
            if (element.ValueKind == JsonValueKind.Array)
            {
                return element.GetArrayLength() == 0; // Array vacío
            }
            
            if (element.ValueKind == JsonValueKind.String)
            {
                return string.IsNullOrEmpty(element.GetString());
            }
            
            return false; // Otros tipos (número, booleano) no se consideran vacíos
        }
        
        /// <summary>
        /// Método contrario a IsNullOrEmpty para una sintaxis más clara
        /// </summary>
        public static bool HasValue(this JsonElement element)
        {
            return !IsNullOrEmpty(element);
        }
    }
