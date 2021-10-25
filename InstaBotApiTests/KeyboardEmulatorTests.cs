using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using InstaBotApi;
using System.Linq;

namespace InstaBotApiTests
{
    [TestClass]
    public class KeyboardEmulatorTests
    {
        [TestMethod]
        public void InsertText_ShouldCallSendKeysOneByOne_WhenCalled()
        {
            var webElementMock = new Mock<IWebElement>();
            var inputText = "some text";

            webElementMock.Object.InsertText(inputText);

            webElementMock.Verify(x => x.SendKeys(It.IsAny<string>()), Times.Exactly(inputText.Length));
            inputText.ToCharArray().ToList()
                .ForEach(x => webElementMock.Verify(mock => mock.SendKeys(x.ToString())));
        }
    }
}
