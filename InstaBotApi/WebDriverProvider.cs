using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace InstaBotApi
{
    static class WebDriverProvider
    {
        public static IWebDriver WebDriver { get; private set; } = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

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
