using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace InstaBot
{
    enum WaitingPeriod
    {
        Short,
        Medium,
        Long
    }

    public class MainWindowViewModel : NotifiableObject
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public bool IsEditingMode { get; set; }
        public ObservableCollection<TagSpecsViewModel> TagSpecs { get; set; }
        private Thread _botRunningThread;
        private IWebDriver _webDriver;

        public MainWindowViewModel()
        {
            StartCommand = new RelayCommand(StartRunning);
            StopCommand = new RelayCommand(StopRunning);
            TagSpecs = new ObservableCollection<TagSpecsViewModel>();
            IsEditingMode = true;
        }

        private void StartRunning(object obj)
        {
            IsEditingMode = false;
            _botRunningThread = new Thread(RunBot);
            _botRunningThread.Start();
        }

        private void RunBot()
        {
            _webDriver = new ChromeDriver();

            LogIntoInstagram();

            foreach (var spec in TagSpecs)
            {
                LikePostsAccordingToSpecs(spec);
            }

            CloseWebDriver();
            IsEditingMode = true;
        }

        private void LikePostsAccordingToSpecs(TagSpecsViewModel tagSpecs)
        {
            int likesNumber = 0;
            if (tagSpecs.TagName != "" && int.TryParse(tagSpecs.LikesNumber, out likesNumber))
            {
                SearchHashtag(tagSpecs.TagName);
                LikeSomePosts(likesNumber);
            }
        }

        void LogIntoInstagram()
        {
            _webDriver.Navigate().GoToUrl("https://www.instagram.com/accounts/login/");

            WaitSomeTime();

            var userName = _webDriver.FindElement(By.Name("username"));
            var password = _webDriver.FindElement(By.Name("password"));
            var loginButton = _webDriver.FindElement(By.ClassName("KUBKM"));

            userName.SendKeys("user");
            password.SendKeys("pass");

            WaitSomeTime(WaitingPeriod.Short);
            loginButton.Submit();
            WaitSomeTime();
        }

        void SearchHashtag(string tag)
        {
            var hashtag = tag.StartsWith("#") ? tag : "#" + tag;

            var searchBar = _webDriver.FindElement(By.ClassName("XTCLo"));

            searchBar.SendKeys(hashtag);
            WaitSomeTime();

            searchBar.SendKeys(Keys.Return);
            searchBar.SendKeys(Keys.Return);

            WaitSomeTime();
        }

        void LikeSomePosts(int likesNumber)
        {
            var posts = _webDriver.FindElements(By.ClassName("_9AhH0"));

            foreach (var post in posts)
            {
                LikePost(post);
                WaitSomeTime(WaitingPeriod.Short);
                if (--likesNumber == 0)
                    return;
            }
        }

        void LikePost(IWebElement post)
        {
            post.Click();
            WaitSomeTime(WaitingPeriod.Short);

            try
            {
                var heart = _webDriver.FindElement(By.ClassName("glyphsSpriteHeart__outline__24__grey_9"));
                heart.Click();
            }
            catch { /*this means that the heart is already red*/ }
            finally
            {
                WaitSomeTime(WaitingPeriod.Short);
            }

            var closePhotoButton = _webDriver.FindElement(By.ClassName("ckWGn"));
            closePhotoButton.Click();
            WaitSomeTime(WaitingPeriod.Short);
        }

        void WaitSomeTime(WaitingPeriod waitingPeriod = WaitingPeriod.Medium)
        {
            switch (waitingPeriod)
            {
                case WaitingPeriod.Short:
                    Thread.Sleep(500);
                    break;
                case WaitingPeriod.Medium:
                    Thread.Sleep(3000);
                    break;
                case WaitingPeriod.Long:
                    Thread.Sleep(6000);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void StopRunning(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            _botRunningThread.Abort();
            CloseWebDriver();
            Mouse.OverrideCursor = null;
            IsEditingMode = true;
        }

        private void CloseWebDriver()
        {
            if (_webDriver == null)
                return;

            _webDriver.Close();
            _webDriver.Dispose();
            _webDriver = null;
        }
    }
}
