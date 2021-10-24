using InstaBotApi.CredentialsHandling;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace InstaBotApi
{
    static class InstagramAuthenticator
    {
        public static void LogIntoInstagram()
        {
            var loginCredentials = CredentialsRepository.ReadCredentials();
            var webDriver = WebDriverProvider.WebDriver;
            webDriver.Navigate().GoToUrl(new Uri("https://www.instagram.com/accounts/login/"));

            ThreadDelayer.WaitSomeTime();

            AcceptCookies(webDriver);

            ThreadDelayer.WaitSomeTime();

            var userName = webDriver.FindElement(By.Name("username"));
            var password = webDriver.FindElement(By.Name("password"));

            userName.InsertText(loginCredentials.Username);
            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            password.InsertText(loginCredentials.Password);

            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);

            var loginButton = webDriver.FindElements(By.TagName("button")).First(x => x.Text.ToLower() == "log in");
            loginButton.Submit();

            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);


            ThreadDelayer.WaitSomeTime();
        }

        private static void AcceptCookies(IWebDriver webDriver)
        {
            var acceptButton = webDriver.FindElements(By.TagName("button")).FirstOrDefault(x => string.Equals(x.Text, "Accept All", StringComparison.OrdinalIgnoreCase));

            if (acceptButton != null)
            {
                acceptButton.Submit();
            }
        }
    }
}
