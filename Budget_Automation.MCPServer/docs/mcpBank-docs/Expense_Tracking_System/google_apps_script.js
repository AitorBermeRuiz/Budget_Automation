// Definición de variables globales
const SHEET_ID = 'TU_ID_DE_HOJA_DE_CALCULO'; // Reemplazar con tu ID de hoja de cálculo
const GASTOS_SHEET_NAME = 'Gastos';
const PRESUPUESTO_SHEET_NAME = 'Presupuesto';
const CATEGORIAS = ['Comida', 'Ropa', 'Coche', 'Inversiones', 'Otros'];

// Función principal para recibir peticiones web
function doPost(e) {
  try {
    const data = JSON.parse(e.postData.contents);
    const action = data.action;
    
    let result;
    switch (action) {
      case 'addGasto':
        result = addGasto(data);
        break;
      case 'getDashboard':
        result = getDashboard(data);
        break;
      case 'getGastosByMonth':
        result = getGastosByMonth(data);
        break;
      case 'getPresupuesto':
        result = getPresupuesto();
        break;
      default:
        result = { error: 'Acción no reconocida' };
    }
    
    return ContentService
      .createTextOutput(JSON.stringify(result))
      .setMimeType(ContentService.MimeType.JSON);
      
  } catch (error) {
    return ContentService
      .createTextOutput(JSON.stringify({ error: error.toString() }))
      .setMimeType(ContentService.MimeType.JSON);
  }
}

// Función para acceso por GET (pruebas/debug)
function doGet(e) {
  return ContentService
    .createTextOutput(JSON.stringify({ status: 'API funcionando' }))
    .setMimeType(ContentService.MimeType.JSON);
}

// Función para añadir un nuevo gasto
function addGasto(data) {
  try {
    const ss = SpreadsheetApp.openById(SHEET_ID);
    const sheet = ss.getSheetByName(GASTOS_SHEET_NAME);
    
    // Validar datos
    if (!data.importe) {
      return { error: 'El importe es obligatorio' };
    }
    
    const importe = parseFloat(data.importe);
    if (isNaN(importe)) {
      return { error: 'El importe debe ser un número válido' };
    }
    
    // Procesar fecha
    let fecha;
    if (data.fecha) {
      fecha = new Date(data.fecha);
      if (isNaN(fecha.getTime())) {
        fecha = new Date(); // Si la fecha no es válida, usar hoy
      }
    } else {
      fecha = new Date();
    }
    
    // Procesar tipo
    const tipo = data.tipo && ['Ingreso', 'Gasto'].includes(data.tipo) ? data.tipo : 'Gasto';
    
    // Procesar categoría
    let categoria = data.categoria;
    if (!categoria || !CATEGORIAS.includes(categoria)) {
      categoria = 'Otros';
    }
    
    // Calcular mes y año
    const mes = Utilities.formatDate(fecha, Session.getScriptTimeZone(), "MMMM");
    const anio = fecha.getFullYear();
    
    // Preparar datos para insertar
    const rowData = [
      fecha,
      tipo,
      categoria,
      importe,
      data.notas || '',
      mes,
      anio,
      sheet.getLastRow() // ID simple basado en número de fila
    ];
    
    // Insertar en la hoja
    sheet.appendRow(rowData);
    
    // Comprobar presupuesto y devolver advertencia si aplica
    let advertenciasPresupuesto = [];
    if (tipo === 'Gasto') {
      const presupuestoData = getPresupuesto();
      const presupuestoCategoria = presupuestoData.find(p => p.categoria === categoria);
      
      if (presupuestoCategoria) {
        const gastosMes = getGastosByCategory(categoria, mes, anio);
        const totalGastoCategoria = gastosMes.reduce((total, g) => total + g.importe, 0) + importe;
        
        if (totalGastoCategoria > presupuestoCategoria.presupuesto) {
          advertenciasPresupuesto.push({
            categoria: categoria,
            presupuesto: presupuestoCategoria.presupuesto,
            gastado: totalGastoCategoria,
            exceso: totalGastoCategoria - presupuestoCategoria.presupuesto
          });
        }
      }
    }
    
    return { 
      success: true, 
      message: 'Gasto registrado correctamente',
      id: sheet.getLastRow(),
      advertencias: advertenciasPresupuesto
    };
    
  } catch (error) {
    return { error: error.toString() };
  }
}

// Función para obtener gastos por mes
function getGastosByMonth(data) {
  try {
    const ss = SpreadsheetApp.openById(SHEET_ID);
    const sheet = ss.getSheetByName(GASTOS_SHEET_NAME);
    
    const mes = data.mes || Utilities.formatDate(new Date(), Session.getScriptTimeZone(), "MMMM");
    const anio = data.anio || new Date().getFullYear();
    
    const dataRange = sheet.getDataRange();
    const values = dataRange.getValues();
    
    // Obtener índices de columnas
    const headers = values[0];
    const fechaIndex = 0;
    const tipoIndex = 1;
    const categoriaIndex = 2;
    const importeIndex = 3;
    const notasIndex = 4;
    const mesIndex = 5;
    const anioIndex = 6;
    
    // Filtrar por mes y año
    const gastos = [];
    for (let i = 1; i < values.length; i++) {
      const row = values[i];
      if (row[mesIndex] === mes && row[anioIndex] == anio) {
        gastos.push({
          fecha: Utilities.formatDate(new Date(row[fechaIndex]), Session.getScriptTimeZone(), "yyyy-MM-dd"),
          tipo: row[tipoIndex],
          categoria: row[categoriaIndex],
          importe: row[importeIndex],
          notas: row[notasIndex]
        });
      }
    }
    
    return gastos;
    
  } catch (error) {
    return { error: error.toString() };
  }
}

// Función auxiliar para obtener gastos por categoría
function getGastosByCategory(categoria, mes, anio) {
  try {
    const ss = SpreadsheetApp.openById(SHEET_ID);
    const sheet = ss.getSheetByName(GASTOS_SHEET_NAME);
    
    const dataRange = sheet.getDataRange();
    const values = dataRange.getValues();
    
    // Filtrar por categoría, mes y año
    const gastos = [];
    for (let i = 1; i < values.length; i++) {
      const row = values[i];
      if (row[2] === categoria && row[5] === mes && row[6] == anio && row[1] === 'Gasto') {
        gastos.push({
          fecha: Utilities.formatDate(new Date(row[0]), Session.getScriptTimeZone(), "yyyy-MM-dd"),
          importe: row[3],
          notas: row[4]
        });
      }
    }
    
    return gastos;
    
  } catch (error) {
    Logger.log('Error en getGastosByCategory: ' + error);
    return [];
  }
}

// Función para obtener el presupuesto
function getPresupuesto() {
  try {
    const ss = SpreadsheetApp.openById(SHEET_ID);
    const sheet = ss.getSheetByName(PRESUPUESTO_SHEET_NAME);
    
    const dataRange = sheet.getDataRange();
    const values = dataRange.getValues();
    
    const presupuestos = [];
    // Omitir encabezado
    for (let i = 1; i < values.length; i++) {
      const row = values[i];
      presupuestos.push({
        categoria: row[0],
        presupuesto: row[1],
        periodo: row[2] || 'Todos' // Opcional: año-mes específico
      });
    }
    
    return presupuestos;
    
  } catch (error) {
    return { error: error.toString() };
  }
}

// Función para obtener dashboard completo
function getDashboard(data) {
  try {
    const mes = data.mes || Utilities.formatDate(new Date(), Session.getScriptTimeZone(), "MMMM");
    const anio = data.anio || new Date().getFullYear();
    
    // Obtener gastos del mes
    const gastos = getGastosByMonth({ mes: mes, anio: anio });
    
    // Obtener presupuestos
    const presupuestos = getPresupuesto();
    
    // Calcular totales por categoría
    const totalesPorCategoria = {};
    let totalGastos = 0;
    let totalIngresos = 0;
    
    CATEGORIAS.forEach(cat => {
      totalesPorCategoria[cat] = 0;
    });
    
    gastos.forEach(gasto => {
      if (gasto.tipo === 'Gasto') {
        totalesPorCategoria[gasto.categoria] += gasto.importe;
        totalGastos += gasto.importe;
      } else {
        totalIngresos += gasto.importe;
      }
    });
    
    // Comparar con presupuestos
    const comparativaPresupuesto = [];
    Object.keys(totalesPorCategoria).forEach(categoria => {
      const presupuesto = presupuestos.find(p => p.categoria === categoria);
      let presupuestoValor = 0;
      
      if (presupuesto) {
        presupuestoValor = presupuesto.presupuesto;
      }
      
      comparativaPresupuesto.push({
        categoria: categoria,
        gastado: totalesPorCategoria[categoria],
        presupuesto: presupuestoValor,
        diferencia: presupuestoValor - totalesPorCategoria[categoria],
        porcentaje: presupuestoValor > 0 ? (totalesPorCategoria[categoria] / presupuestoValor) * 100 : 0
      });
    });
    
    return {
      periodo: { mes: mes, anio: anio },
      resumen: {
        totalGastos: totalGastos,
        totalIngresos: totalIngresos,
        balance: totalIngresos - totalGastos
      },
      gastosPorCategoria: totalesPorCategoria,
      comparativaPresupuesto: comparativaPresupuesto,
      ultimosMovimientos: gastos.slice(0, 5) // Últimos 5 movimientos
    };
    
  } catch (error) {
    return { error: error.toString() };
  }
}

// Función para procesar correos automáticamente
function procesarCorreosGastos() {
  try {
    // Buscar correos no leídos con etiqueta específica
    const threads = GmailApp.search('label:gastos is:unread');
    
    let procesados = 0;
    let errores = 0;
    
    for (let i = 0; i < threads.length; i++) {
      const thread = threads[i];
      const messages = thread.getMessages();
      
      for (let j = 0; j < messages.length; j++) {
        const message = messages[j];
        
        try {
          // Extraer información del correo
          const asunto = message.getSubject();
          const cuerpo = message.getPlainBody();
          
          // Aquí implementar la lógica de extracción según tus necesidades
          // Este es un ejemplo muy básico que busca patrones en el asunto o cuerpo
          
          let importe = null;
          let categoria = null;
          let notas = null;
          
          // Buscar importe (número seguido de € en asunto o cuerpo)
          const importeRegex = /(\d+(?:[.,]\d+)?)[\s]*€/;
          const importeMatch = asunto.match(importeRegex) || cuerpo.match(importeRegex);
          
          if (importeMatch) {
            importe = parseFloat(importeMatch[1].replace(',', '.'));
          }
          
          // Intentar determinar la categoría basada en palabras clave
          if (asunto.match(/restaurant|comida|café|cafetería|menú|cena/i) || 
              cuerpo.match(/restaurant|comida|café|cafetería|menú|cena/i)) {
            categoria = 'Comida';
          } else if (asunto.match(/ropa|vestir|zapatería|moda/i) || 
                     cuerpo.match(/ropa|vestir|zapatería|moda/i)) {
            categoria = 'Ropa';
          } else if (asunto.match(/coche|gasolina|gasolinera|taller|parking/i) || 
                     cuerpo.match(/coche|gasolina|gasolinera|taller|parking/i)) {
            categoria = 'Coche';
          } else if (asunto.match(/inver|fondo|bolsa|acción/i) || 
                     cuerpo.match(/inver|fondo|bolsa|acción/i)) {
            categoria = 'Inversiones';
          } else {
            categoria = 'Otros';
          }
          
          // Extraer notas (usar el asunto como notas)
          notas = asunto;
          
          // Si hemos podido extraer al menos el importe, registrar el gasto
          if (importe !== null) {
            const result = addGasto({
              importe: importe,
              categoria: categoria,
              tipo: 'Gasto',
              notas: notas,
              fecha: message.getDate()
            });
            
            if (result.error) {
              errores++;
              Logger.log('Error al procesar correo: ' + result.error);
            } else {
              procesados++;
            }
          } else {
            Logger.log('No se pudo extraer el importe del correo: ' + asunto);
          }
          
          // Marcar como leído
          message.markRead();
          
        } catch (msgError) {
          Logger.log('Error procesando mensaje: ' + msgError);
          errores++;
        }
      }
      
      // Quitar la etiqueta de gastos para no procesarlo nuevamente
      thread.removeLabel(GmailApp.getUserLabelByName('gastos'));
    }
    
    return {
      procesados: procesados,
      errores: errores,
      timestamp: new Date().toISOString()
    };
    
  } catch (error) {
    Logger.log('Error general en procesarCorreosGastos: ' + error);
    return {
      procesados: 0,
      errores: 1,
      error: error.toString(),
      timestamp: new Date().toISOString()
    };
  }
}

// Configurar un disparador para ejecutar automáticamente
function configurarDisparador() {
  // Eliminar disparadores existentes para evitar duplicados
  const disparadores = ScriptApp.getProjectTriggers();
  for (let i = 0; i < disparadores.length; i++) {
    if (disparadores[i].getHandlerFunction() === 'procesarCorreosGastos') {
      ScriptApp.deleteTrigger(disparadores[i]);
    }
  }
  
  // Crear un nuevo disparador para ejecutar cada hora
  ScriptApp.newTrigger('procesarCorreosGastos')
    .timeBased()
    .everyHours(1)
    .create();
}
