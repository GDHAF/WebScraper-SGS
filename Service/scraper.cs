using System.Threading.Tasks;
using OpenQA.Selenium;
using Models;
using CsvHelper;
using System.Globalization;

namespace Service
{
    public static class Scraper
    {
        public static async Task AddSerie(string code, IWebDriver driver)
        {
            await Task.Delay(2000); // Aguarda 2 segundos para a página carregar completamente
            driver.FindElement(By.XPath("//*[@id='txCodigo']")).SendKeys(code + Keys.Enter);
            await Task.Delay(2000); // Aguarda 2 segundos para a página carregar completamente
            driver.FindElement(By.XPath("//*[@id='botaoMarcar']/input")).Click();
            await Task.Delay(1000);
            driver.FindElement(By.XPath("//*[@id='botaoAcrescentar']/input")).Click();
        }

        public static async Task ClearInput(IWebDriver driver)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(600); // Aguarda 600ms para a página carregar completamente
                driver.FindElement(By.XPath("//*[@id='txCodigo']")).SendKeys(Keys.Backspace);
            }
        }

        public static void saveCsv(SeriesInfo serie, List<SeriesValues> list_values)
        {
            using (var writer = new StreamWriter("Output/" + serie.OutputFile))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
			{ 
				csv.WriteRecords(list_values); 
			}
				
        }

        public static SeriesValues save_row(IWebElement row, int i, SeriesInfo serie, int s)
        {
            var data = row.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[" + i + "]/td[1]/div/span"));
			var valueElement = row.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[" + i + "]/td[" + (s + 1) + "]/div/span"));

			// Criar um novo objeto e adicionar à lista
			var serie_value = new Models.SeriesValues { data = data.Text, value = valueElement.Text == "-" ? 0 : decimal.Parse(valueElement.Text) };
            return serie_value;
        }
    }
}
