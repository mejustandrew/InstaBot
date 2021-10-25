using OpenQA.Selenium;

namespace InstaBotApi
{
    public static class KeyboardEmulator
    {
        public static void InsertText(this IWebElement targetElement, string inputText)
        {
            foreach(var c in inputText)
            {
                ThreadDelayer.WaitSomeTime(WaitingPeriod.VeryShort);
                targetElement.SendKeys(c.ToString());
            }
        }
    }
}
