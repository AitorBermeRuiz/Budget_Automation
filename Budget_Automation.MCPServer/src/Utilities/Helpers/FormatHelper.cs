using System.Globalization;

namespace Budget_Automation.MCPServer.Utilities.Helpers
{
    public static class FormatHelper
    {
        /// <summary>
        /// Formatea un número decimal utilizando la cultura invariante.
        /// </summary>
        /// <param name="value">El valor decimal a formatear.</param>
        /// <returns>El valor formateado como cadena.</returns>
        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
