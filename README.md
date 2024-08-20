## Extractor de Tasas de Cambio y INPC del BCV

Este script de C# se encarga de extraer las tasas de cambio del dólar y el euro, así como el Índice Nacional de Precios al Consumidor (INPC) del sitio web del Banco Central de Venezuela.

### Funcionamiento General
1. **Descarga la página:** Utiliza la librería `HtmlAgilityPack` para descargar el HTML del sitio web del BCV.
2. **Análisis del HTML:** Emplea XPath para navegar por el DOM (Document Object Model) y localizar los nodos que contienen las tasas de cambio y el INPC.
3. **Extracción de datos:** Obtiene el texto de los nodos encontrados y lo limpia para eliminar caracteres no numéricos.
4. **Conversión a números:** Convierte los textos extraídos a números decimales.
5. **Mostrar resultados:** Imprime las tasas de cambio y el INPC en la consola.
