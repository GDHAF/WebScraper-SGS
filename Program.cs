using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;

namespace scraper_sgs 
{ 
	class Program 
	{ 

		public class Serie 
		{ 
			public string data { get; set; } 
			public decimal value { get; set; } 
		}

		static void Main(string[] args) 
		{ 

			var updates = new List<Serie>();

			var web = new HtmlWeb();

            var documento = web.Load("https://www3.bcb.gov.br/sgspub/consultarvalores/consultarValoresSeries.do?method=consultarValores");
		
			var series = documento.DocumentNode.QuerySelectorAll("tr.fundoPadraoAClaro3");
			var series2 = documento.DocumentNode.QuerySelectorAll("tr.fundoPadraoAClaro2");

			foreach (var serie in series) 
			{ 
				var data = HtmlEntity.DeEntitize(serie.QuerySelector("span.textoPequeno").InnerText); 
				var value = HtmlEntity.DeEntitize(serie.QuerySelector("span.textoPequeno").InnerText); 

				updates.Add(new Serie { data = data, value = decimal.Parse(value) }); 
			}

			foreach (var serie in series2) 
			{ 
				var data = HtmlEntity.DeEntitize(serie.QuerySelector("span.textoPequeno").InnerText); 
				var value = HtmlEntity.DeEntitize(serie.QuerySelector("span.textoPequeno").InnerText); 

				updates.Add(new Serie { data = data, value = decimal.Parse(value) }); 
			}

			using (var writer = new StreamWriter("serie.csv"))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
			{ 
				csv.WriteRecords(updates); 
			}

		} 
	} 
}