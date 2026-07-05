using CsvHelper;
using System.Globalization;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Models;
using Service;

namespace scraper_sgs 
{ 
	class Program 
	{ 

		static async Task Main(string[] args) 
		{ 		
			
			List<dicionario> dicionario = new List<dicionario>();
			
			List<SeriesInfo> series = new List<SeriesInfo>
			{
				new()
				{
					num = 1,
					id_unico = 20553,
					Name = "Saldo PJ",
					Description = "Vehicle financing balance for companies",
					OutputFile = "CreditBalanceCompanies.csv"	
				},
				new()
				{
					num = 2,
					id_unico = 20581,
					Name = "Saldo PF",
					Description = "Vehicle financing balance for households",
					OutputFile = "CreditBalanceHouseholds.csv"
				},

				new()
				{
					num = 3,
					id_unico = 20673,
					Name = "Concessões PF",
					Description = "New vehicle financing concessions",
					OutputFile = "CreditConcessionsHouseholds.csv"
				},
				new()
				{
					num = 4,
					id_unico = 20728,
					Name = "Juros PJ",
					Description = "Average annual interest rate",
					OutputFile = "InterestRatesCompanies.csv"
				},
				new()
				{
					num = 5,
					id_unico = 20749,
					Name = "Juros PF",
					Description = "Average annual interest rate",
					OutputFile = "InterestRatesHouseholds.csv"
				},
				new()
				{
					num = 6,
					id_unico = 21084,
					Name = "Inadimplência  PF",
					Description = "Percent of 90 days past due loans for households",
					OutputFile = "NonPerformingLoansHouseholds.csv"
				}
			};

			// Inicializa o WebDriver (abre o Chrome)
			IWebDriver driver = new ChromeDriver();

			// Navega para uma URL específica
			driver.Navigate().GoToUrl("https://www3.bcb.gov.br/sgspub/");

			// Maximiza a tela
			driver.Manage().Window.Maximize();
			

			foreach(var serie in series)
			{
				await Scraper.AddSerie(serie.id_unico.ToString(), driver);
				await Scraper.ClearInput(driver);
			}

			driver.FindElement(By.XPath("/html/body/form/center/span/center/table/tbody/tr/td[4]/div/input")).Click();

			await Task.Delay(2000);
			driver.FindElement(By.XPath("/html/body/center/form/div[2]/input[2]")).Click();

            var pageSource = driver.PageSource;

			var updateRow = driver.FindElements(By.XPath("//*[@id='valoresSeries']/tbody/tr"));

			var n = Math.Ceiling((double)int.Parse(driver.FindElement(By.XPath("//*[@id='valoresSeries\']/tbody/tr[1]/td/span/span[1]/b")).Text)/103);

			Console.WriteLine("Total pages: " + n);

			foreach(var serie in series)
			{
				if(serie.num != 1){
					driver.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[1]/td/span/span[2]/div/table/tbody/tr/td/a[1]")).Click();
				}

				Console.WriteLine("Processing series: " + serie.Name);
				var list_values = new List<Models.SeriesValues>();

				for (int p = 1; p <= n+1; p++){
					Console.WriteLine("Processing page: " + p);
					pageSource = driver.PageSource;
					updateRow = driver.FindElements(By.XPath("//*[@id='valoresSeries']/tbody/tr"));
					await Task.Delay(2000); // Aguarda 2 segundos para a página carregar completamente
					//Começando pela linha 4, pois as 3 primeiras linhas são cabeçalhos
					int i = 4;

					foreach (var row in updateRow)
					{
						try{
							list_values.Add(Scraper.save_row(row, i, serie));
							i+=1;
						}catch(OpenQA.Selenium.NoSuchElementException)
						{
							//Finalizar o loop caso não haja mais linhas na tabela
							break; 
						}
					}

					try
					{
						if(p == 1)
						{
							driver.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[1]/td/span/span[2]/div/table/tbody/tr/td/a[" + p + "]")).Click();
						}
						else
						{
							int aux = p+2;
							Console.WriteLine(driver.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[1]/td/span/span[2]/div/table/tbody/tr/td/a[" + aux + "]")).Text);
							driver.FindElement(By.XPath("//*[@id='valoresSeries']/tbody/tr[1]/td/span/span[2]/div/table/tbody/tr/td/a[" + aux + "]")).Click();
						}
					}catch(OpenQA.Selenium.NoSuchElementException)
					{
						// Encerrar o loop se não houver mais páginas para processar
						break;
					}		
			
				}

				// Salvar a lista de valores em um arquivo CSV usando CsvHelper
				Scraper.saveCsv(serie, list_values);

                dicionario.Add(new dicionario { id_unico = serie.OutputFile, description = serie.Description, file_path = "Output/" + serie.OutputFile });
				
			}

			//Finaizar salvando o dicionário de séries em um arquivo CSV
			using (var writer = new StreamWriter("Output/dicionario.csv"))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
			{ 
				csv.WriteRecords(dicionario); 
			}

		} 
	} 
}