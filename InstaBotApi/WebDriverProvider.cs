using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace InstaBotApi
{
    static class WebDriverProvider
    {
        public static IWebDriver WebDriver
        {
            get
            {
                if (_webDriver == null)
                    _webDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

                return _webDriver;
            }
            private set
            {
                _webDriver = value;
            }
        }
        private static IWebDriver _webDriver;

        public static void CloseWebDriver()
        {
            if (WebDriver == null)
                return;

            WebDriver.Quit();
            WebDriver.Dispose();
            WebDriver = null;
        }
    }
}
