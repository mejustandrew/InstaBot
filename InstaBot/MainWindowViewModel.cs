using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;

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
        public bool IsBotRunning { get; set; }
        public ObservableCollection<TagSpecsViewModel> TagSpecs { get; set; }
        public bool RenewCredentialsOnStart { get; set; }

        private Thread _botRunningThread;
        private IWebDriver _webDriver;
        private bool _isEditingMode;
        private bool _shouldRun;
        private Credentials _loginCredentials;

        public bool IsEditingMode
        {
            get => _isEditingMode;
            set
            {
                _isEditingMode = value;
                IsBotRunning = !_isEditingMode;
            }
        }

        public MainWindowViewModel()
        {
            StartCommand = new RelayCommand(StartRunning);
            StopCommand = new RelayCommand(StopRunning);
            var specs = SettingsRepository.ReadTagSettings();
            TagSpecs = new ObservableCollection<TagSpecsViewModel>(specs);
            IsEditingMode = true;
            _loginCredentials = CredentialsManager.GetLoginData();
        }

        private void StartRunning(object obj)
        {
            SettingsRepository.SaveTagSettings(TagSpecs.ToList());

            _shouldRun = true;
            IsEditingMode = false;
            if (RenewCredentialsOnStart)
                _loginCredentials = CredentialsManager.RenewCredentials();

            _botRunningThread = new Thread(RunBot);
            _botRunningThread.Start();
        }

        private void RunBot()
        {
            _webDriver = new ChromeDriver();

            LogIntoInstagram();
            HandleNotificationsSetting();
            while(_shouldRun)
            {
                foreach (var spec in TagSpecs)
                {
                    LikePostsAccordingToSpecs(spec);
                }
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

        private void LogIntoInstagram()
        {
            _webDriver.Navigate().GoToUrl("https://www.instagram.com/accounts/login/");

            WaitSomeTime();

            var userName = _webDriver.FindElement(By.Name("username"));
            var password = _webDriver.FindElement(By.Name("password"));
            var loginButton = _webDriver.FindElement(By.XPath("//button[text()=\"Log in\"]"));

            userName.SendKeys(_loginCredentials.Username);
            WaitSomeTime(WaitingPeriod.Short);
            password.SendKeys(_loginCredentials.Password);

            WaitSomeTime(WaitingPeriod.Short);
            loginButton.Submit();

            WaitSomeTime();
        }

        private void HandleNotificationsSetting()
        {
            try
            {
                var notNow = _webDriver.FindElement(By.XPath("//button[text()=\"Not Now\"]"));
                notNow.Click();
                WaitSomeTime();
            }
            catch
            {
            }
        }

        private void SearchHashtag(string tag)
        {
            var hashtag = tag.StartsWith("#") ? tag : "#" + tag;

            var searchBar = _webDriver.FindElement(By.ClassName("XTCLo"));

            searchBar.SendKeys(hashtag);
            WaitSomeTime();

            searchBar.SendKeys(Keys.Return);
            searchBar.SendKeys(Keys.Return);

            WaitSomeTime();
        }

        private void LikeSomePosts(int postsNumber)
        {
            ScrollDown();
            var popularPostsNumber = 9;
            var likesGiven = popularPostsNumber;
            postsNumber += popularPostsNumber;

            var posts = _webDriver.FindElements(By.ClassName("_9AhH0"));

            while (likesGiven < postsNumber)
            {
                try
                {
                    while (likesGiven < posts.Count && likesGiven < postsNumber)
                    {
                        LikePost(posts[likesGiven]);
                        WaitSomeTime(WaitingPeriod.Short);
                        likesGiven++;
                    }
                    return;
                }
                catch (StaleElementReferenceException ex)
                {
                    posts = _webDriver.FindElements(By.ClassName("_9AhH0"));
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        private void ScrollDown()
        {
            for (int i = 0; i < 50; i++)
            {
                ((IJavaScriptExecutor)_webDriver).ExecuteScript("scrollBy(0,1000)");
                WaitSomeTime(WaitingPeriod.Short);
            }
        }

        private void LikePost(IWebElement post)
        {
            OpenPost(post);
            ClickHeart();
            ClosePost();
        }

        private void OpenPost(IWebElement post)
        {
            try
            {
                post.Click();
                WaitSomeTime();
            }
            catch (Exception ex)
            {
            }
        }

        private void ClickHeart()
        {
            try
            {
                var heart = _webDriver.FindElement(By.ClassName("coreSpriteHeartOpen"));
                var heartSpan = heart.FindElement(By.XPath("./span"));
                var likeText = heartSpan.GetAttribute("aria-label");

                if (likeText.ToLower() != "unlike")
                    heart.Click();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                WaitSomeTime(WaitingPeriod.Short);
            }
        }

        private void ClosePost()
        {
            try
            {
                var closePhotoButton = _webDriver.FindElement(By.ClassName("ckWGn"));
                closePhotoButton.Click();
                WaitSomeTime(WaitingPeriod.Short);
            }
            catch
            {
            }
        }

        private void WaitSomeTime(WaitingPeriod waitingPeriod = WaitingPeriod.Medium)
        {
            int delayFactor = 2;
            switch (waitingPeriod)
            {
                case WaitingPeriod.Short:
                    Thread.Sleep(delayFactor * 500);
                    break;
                case WaitingPeriod.Medium:
                    Thread.Sleep(delayFactor * 3000);
                    break;
                case WaitingPeriod.Long:
                    Thread.Sleep(delayFactor * 6000);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void StopRunning(object obj)
        {
            _shouldRun = false;
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
