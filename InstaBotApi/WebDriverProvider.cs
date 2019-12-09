using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace InstaBotApi
{
    static class WebDriverProvider
    {
        public static IWebDriver WebDriver { get; private set; } = new ChromeDriver();

        public static void CloseWebDriver()
        {
            if (WebDriver == null)
                return;

            WebDriver.Close();
            WebDriver.Dispose();
            WebDriver = null;
        }
    }
}
