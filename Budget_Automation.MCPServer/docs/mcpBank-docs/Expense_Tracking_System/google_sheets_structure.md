# Google Sheets Structure

## Hoja 1: Gastos (Expenses)

| Campo | Tipo | Descripción | Fórmula (si aplica) |
|-------|------|-------------|---------------------|
| Fecha | Fecha | Fecha de la transacción | Predeterminado: =HOY() |
| Tipo | Desplegable | "Ingreso" o "Gasto" | Validación de datos |
| Categoría | Desplegable | Categorías predefinidas | Validación de datos con rango de categorías |
| Importe | Moneda | Cantidad con signo € | Formato personalizado |
| Notas | Texto | Detalles adicionales | - |
| Mes | Calculado | Nombre del mes | =TEXTO(A2;"MMMM") |
| Año | Calculado | Año de la transacción | =AÑO(A2) |
| ID | Autonumérico | Identificador único | =FILA()-1 |

### Configuración de validación de datos:
- **Tipo**: Lista con valores "Ingreso" y "Gasto"
- **Categoría**: Lista con valores "Comida", "Ropa", "Coche", "Inversiones", "Otros"

## Hoja 2: Presupuesto (Budget)

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Categoría | Texto | Igual que las categorías de la hoja Gastos |
| Presupuesto | Moneda | Cantidad asignada mensualmente |
| Año-Mes | Texto | Formato "AAAA-MM" para presupuestos variables |

## Hoja 3: Dashboard

### Sección 1: Resumen del mes actual
```
=SUMIFS(Gastos!D:D; Gastos!B:B; "Gasto"; Gastos!F:F; TEXTO(HOY();"MMMM"); Gastos!G:G; AÑO(HOY()))
```

### Sección 2: Gastos por categoría (mes actual)
```
=SUMIFS(Gastos!D:D; Gastos!B:B; "Gasto"; Gastos!C:C; "Comida"; Gastos!F:F; TEXTO(HOY();"MMMM"); Gastos!G:G; AÑO(HOY()))
```

### Sección 3: Comparativa con presupuesto
```
=BUSCARV("Comida";Presupuesto!A:B;2;FALSO)-SUMIFS(Gastos!D:D; Gastos!B:B; "Gasto"; Gastos!C:C; "Comida"; Gastos!F:F; TEXTO(HOY();"MMMM"); Gastos!G:G; AÑO(HOY()))
```

### Sección 4: Gráficos recomendados
- Gráfico circular de gastos por categoría
- Gráfico de barras de comparación presupuesto vs. gasto real
- Gráfico de líneas de evolución de gastos a lo largo del tiempo
