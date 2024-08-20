using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BCVTasa
{
    partial class Program
    {
        [GeneratedRegex(@"[^\d,\.]")]
        private static partial Regex TasaRegex();

        static void Main(string[] args)
        {
            string url = "https://www.bcv.org.ve/";

            // Crear un objeto HtmlWeb para descargar la página
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            #region Buscar tasas del Dolar y Euro
            // Buscar el elemento que contiene la tasa del Banco Central de Venezuela
            HtmlNode dolarNode = doc.DocumentNode.SelectSingleNode("//div[@id='dolar']");
            HtmlNode euroNode = doc.DocumentNode.SelectSingleNode("//div[@id='euro']");

            string tasaBCVDolar = dolarNode.InnerText.Trim();
            string tasaBCVEuro = euroNode.InnerText.Trim();

            decimal tasaDolar = 0;
            decimal tasaEuro = 0;

            if (tasaBCVDolar != null)
            {
                string cleanTasaDolar = TasaRegex().Replace(tasaBCVDolar, "");
                tasaDolar = decimal.Parse(cleanTasaDolar.Replace('.', ','));
                Console.WriteLine("Tasa del Dólar: " + tasaDolar);
            }
            else
            {
                Console.WriteLine("No se encontró el div con la tasa del dólar.");
            }

            if (tasaBCVEuro != null)
            {
                string cleanTasaEuro = TasaRegex().Replace(tasaBCVEuro, "");
                tasaEuro = decimal.Parse(cleanTasaEuro.Replace('.', ','));
                Console.WriteLine("Tasa del Euro: " + tasaEuro);
            }
            else
            {
                Console.WriteLine("No se encontró el div con la tasa del euro.");
            }
            #endregion

            #region Buscar INPC
            // Buscar el elemento que contiene el INPC

            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int currentMonth = currentDate.Month;
            decimal inpc = 0;

            HtmlNode inpcNode = doc.DocumentNode.SelectSingleNode("//div[@id='content-block-views-ind-home-inpc-block']//div[contains(@class, 'views-field-field-indic')]//div[@class='field-content']");

            string inpcValue = inpcNode?.InnerText.Trim();

            if (inpcValue != null)
            {
                // Eliminar puntos de miles y reemplazar la coma decimal por un punto decimal
                string cleanInpcValue = Regex.Replace(inpcValue, @"\.", "").Replace(',', '.');
                 inpc = decimal.Parse(cleanInpcValue);
                Console.WriteLine("Índice Nacional de Precios al Consumidor: " + inpc);
            }
            else
            {
                Console.WriteLine("No se encontró el div con el INPC.");
            }
            #endregion
            
        }
    }
}
