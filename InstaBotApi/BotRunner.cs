using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InstaBotApi
{
    public class Tag
    {
        public string TagName { get; set; }
        public string LikesNumber { get; set; }
    }

    public static class BotRunner
    {
        private static Task _botRunningTask;
        private static readonly CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();
        private static object _lockObject = new object();
        private static ILogger _logger;

        public static Task RunBotForTagsAsync(IEnumerable<Tag> tags, ILogger logger)
        {
            _logger = logger;
            lock (_lockObject)
            {
                if (_botRunningTask == null || _botRunningTask.IsCompleted)
                {
                    _botRunningTask = new Task(() => RunBotForTags(tags, _cancelationTokenSource.Token));
                    _botRunningTask.Start();
                    _botRunningTask.ContinueWith(task => WebDriverProvider.CloseWebDriver(), TaskScheduler.Current);
                    return _botRunningTask;
                }
                else
                {
                    _logger.Error("The bot runner cannot be started because the previous bot running task was not completed.");
                    throw new Exception("cannot start another task because the current one is not completed.");
                }
            }
        }

        public static void RunBotForTags(IEnumerable<Tag> tags, CancellationToken cancellationToken)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            _logger.Information("Running for given tags started");

            InstagramAuthenticator.LogIntoInstagram();
            HandleNotificationsSetting();

            foreach (var tag in tags)
            {
                cancellationToken.ThrowIfCancellationRequested();
                LikePostsAccordingToTag(tag);
            }
        }

        private static void LikePostsAccordingToTag(Tag tag)
        {
            int likesNumber;
            if (tag.TagName != "" && int.TryParse(tag.LikesNumber, out likesNumber))
            {
                SearchHashtag(tag.TagName);
                LikeSomePosts(likesNumber);
            }
        }

        private static void HandleNotificationsSetting()
        {
            try
            {
                for(int i=0; i <= 1; i++)
                {
                    _logger.Information("Handling notification settings pop-up");
                    var notNow = WebDriverProvider.WebDriver.FindElement(By.XPath("//button[text()=\"Not Now\"]"));
                    notNow.Click();
                    ThreadDelayer.WaitSomeTime();
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"Exception of type {ex.GetType()} occured when attempting to handle notification settings pop-up. Exception message: {ex.Message}");
            }
        }

        private static void SearchHashtag(string tag)
        {
            _logger.Information($"Searching tag {tag}.");
            var hashtag = tag.StartsWith("#") ? tag : "#" + tag;

            var searchBar = WebDriverProvider.WebDriver.FindElement(By.ClassName("XTCLo"));

            searchBar.InsertText(hashtag);
            ThreadDelayer.WaitSomeTime();

            searchBar.SendKeys(Keys.Return);
            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);

            //searchBar.SendKeys(Keys.Return);

            ThreadDelayer.WaitSomeTime();
        }

        private static void LikeSomePosts(int postsNumber)
        {
            LoadPostsInPage();
            var popularPostsNumber = 9;
            var likesGiven = popularPostsNumber;
            postsNumber += popularPostsNumber;

            var posts = WebDriverProvider.WebDriver.FindElements(By.ClassName("_9AhH0"));


            try
            {
                while (likesGiven < posts.Count && likesGiven < postsNumber)
                {
                    LikePost(posts[likesGiven]);
                    ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
                    likesGiven++;
                }
                return;
            }
            catch (StaleElementReferenceException ex)
            {
                _logger.Error("Element became unavaiable before attemot to open. Reloading elements.");
                posts = WebDriverProvider.WebDriver.FindElements(By.ClassName("_9AhH0"));
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception of type {ex.GetType()} occured and was not handled. Exception message: {ex.Message} Exception stack trace:{ex.StackTrace}");
                return;
            }
        }

        private static void LoadPostsInPage()
        {
            _logger.Information("Loading posts into page.");
            for (int i = 0; i < 50; i++)
            {
                ((IJavaScriptExecutor)WebDriverProvider.WebDriver).ExecuteScript("scrollBy(0,1000)");
                ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            }

            ((IJavaScriptExecutor)WebDriverProvider.WebDriver).ExecuteScript("window.scrollTo(0, 0)");
            ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
        }

        private static void LikePost(IWebElement post)
        {
            OpenPost(post);
            ClickHeart();
            ClosePost();
        }

        private static void OpenPost(IWebElement post)
        {
            try
            {
                post.Click();
                ThreadDelayer.WaitSomeTime();
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception of type {ex.GetType()} occured when attempting to open a post. Exception message: {ex.Message}");
            }
        }

        private static void ClickHeart()
        {
            try
            {
                _logger.Information("Attempting to like");
                var buttons = WebDriverProvider.WebDriver.FindElements(By.TagName("button"));
                var likeButton = buttons.FirstOrDefault(x => x.FindElements(By.TagName("span")) != null && x.FindElements(By.TagName("span"))
                .Any(y => y.GetAttribute("aria-label") == "Like"));

                if (likeButton != null)
                {
                    likeButton.Click();
                    _logger.Information("Like given successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception of type {ex.GetType()} occured when attempting to like. Exception message: {ex.Message}");
            }
            finally
            {
                ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            }
        }

        private static void ClosePost()
        {
            try
            {
                var closePhotoButton = WebDriverProvider.WebDriver.FindElement(By.ClassName("ckWGn"));
                closePhotoButton.Click();
                ThreadDelayer.WaitSomeTime(WaitingPeriod.Short);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception of type {ex.GetType()} occured when attempting close post. Exception message: {ex.Message}");
            }
        }

        public static void StopRunning()
        {
            _cancelationTokenSource.Cancel();
            _logger.Information("Canceled the token on runner.");
            WebDriverProvider.CloseWebDriver();
            _logger.Information("Closed web driver");
        }
    }
}
