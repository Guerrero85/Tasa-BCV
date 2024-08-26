using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BCVTasa
{
    partial class Program
{
    [GeneratedRegex(@"[^\d,\.]")]
    private static partial Regex TasaRegex();

    static async Task Main(string[] args)
    {
        string url = "https://www.bcv.org.ve/";
        string connectionString = "Server=tu_servidor;Database=tu_base_de_datos;User Id=tu_usuario;Password=tu_contraseña;";

        try
        {
            // Crear un objeto HtmlWeb para descargar la página
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Crear tareas para obtener las tasas y el INPC en paralelo
            Task<decimal> tasaDolarTask = GetTasaDolar(doc);
            Task<decimal> tasaEuroTask = GetTasaEuro(doc);
            Task<decimal> inpcTask = GetINPC(doc);

            // Esperar a que todas las tareas finalicen
            await Task.WhenAll(tasaDolarTask, tasaEuroTask, inpcTask);

            // Mostrar los resultados
            Console.WriteLine("Tasa del Dólar: " + tasaDolarTask.Result);
            Console.WriteLine("Tasa del Euro: " + tasaEuroTask.Result);
            Console.WriteLine("Índice Nacional de Precios al Consumidor: " + inpcTask.Result);

            // Guardar los datos en la base de datos
            await GuardarTasa(tasaDolarTask.Result, "USD", connectionString);
            await GuardarTasa(tasaEuroTask.Result, "EUR", connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static async Task<decimal> GetTasaDolar(HtmlDocument doc)
    {
        try
        {
            HtmlNode dolarNode = doc.DocumentNode.SelectSingleNode("//div[@id='dolar']");
            string tasaBCVDolar = dolarNode.InnerText.Trim();

            if (tasaBCVDolar != null)
            {
                string cleanTasaDolar = TasaRegex().Replace(tasaBCVDolar, "");
                return decimal.Parse(cleanTasaDolar.Replace('.', ','));
            }
            else
            {
                Console.WriteLine("No se encontró el div con la tasa del dólar.");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener la tasa del dólar: " + ex.Message);
            return 0;
        }
    }

    static async Task<decimal> GetTasaEuro(HtmlDocument doc)
    {
        try
        {
            HtmlNode euroNode = doc.DocumentNode.SelectSingleNode("//div[@id='euro']");
            string tasaBCVEuro = euroNode.InnerText.Trim();

            if (tasaBCVEuro != null)
            {
                string cleanTasaEuro = TasaRegex().Replace(tasaBCVEuro, "");
                return decimal.Parse(cleanTasaEuro.Replace('.', ','));
            }
            else
            {
                Console.WriteLine("No se encontró el div con la tasa del euro.");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener la tasa del euro: " + ex.Message);
            return 0;
        }
    }

    static async Task<decimal> GetINPC(HtmlDocument doc)
    {
        try
        {
            HtmlNode inpcNode = doc.DocumentNode.SelectSingleNode("//div[@id='content-block-views-ind-home-inpc-block']//div[contains(@class, 'views-field-field-indic')]//div[@class='field-content']");
            string inpcValue = inpcNode?.InnerText.Trim();

            if (inpcValue != null)
            {
                // Eliminar puntos de miles y reemplazar la coma decimal por un punto decimal
                string cleanInpcValue = Regex.Replace(inpcValue, @"\.", "").Replace(',', '.');
                return decimal.Parse(cleanInpcValue);
            }
            else
            {
                Console.WriteLine("No se encontró el div con el INPC.");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener el INPC: " + ex.Message);
            return 0;
        }
    }

    static async Task GuardarTasa(decimal valor, string codigoMoneda, string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Tasa (CodigoMoneda, ValorMoneda, FechaRegistro) VALUES (@codigoMoneda, @valor, GETDATE())";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@codigoMoneda", codigoMoneda);
                command.Parameters.AddWithValue("@valor", valor);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
}
