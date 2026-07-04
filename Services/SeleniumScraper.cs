using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Service
{
    public class SeleniumScraper{
        public static async Task addSerie(string code, IWebDriver driver)
		{
			await Task.Delay(2000); // Aguarda 2 segundos para a página carregar completamente
			driver.FindElement(By.XPath("//*[@id='txCodigo']")).SendKeys(code + Keys.Enter);
			await Task.Delay(2000); // Aguarda 2 segundo para a página carregar completamente
			driver.FindElement(By.XPath("//*[@id='botaoMarcar']/input")).Click();
			await Task.Delay(1000);
			driver.FindElement(By.XPath("//*[@id='botaoAcrescentar']/input")).Click();
		}

		public static async Task clearInput(IWebDriver driver)
		{
			for(int i = 0; i < 5; i++)
			{
				await Task.Delay(600); // Aguarda 1 segundo para a página carregar completamente
				driver.FindElement(By.XPath("//*[@id='txCodigo']")).SendKeys(Keys.Backspace);
			}
		}
    }

}