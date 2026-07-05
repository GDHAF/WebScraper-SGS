using System.Threading.Tasks;
using OpenQA.Selenium;

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

    }
}
