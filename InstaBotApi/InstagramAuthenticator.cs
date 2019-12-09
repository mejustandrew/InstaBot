using InstaBotApi.CredentialsHandling;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

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

            var userName = webDriver.FindElement(By.Name("username"));
            var password = webDriver.FindElement(By.Name("password"));
            var loginButton = webDriver.FindElement(By.XPath("//button[text()=\"Log in\"]"));

            userName.SendKeys(loginCredentials.Username);
            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            password.SendKeys(loginCredentials.Password);

            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            loginButton.Submit();

            ThreadDelayer.WaitSomeTime();
        }
    }
}
