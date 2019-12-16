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
        public string Name { get; set; }
        public string LikesNumber { get; set; }
    }

    public static class BotRunner
    {
        private static Task _botRunningTask;
        private static readonly CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();
        private static object _lockObject = new object();

        public static Task RunBotForTagsAsync(IEnumerable<Tag> tags)
        {
            lock (_lockObject)
            {
                if (_botRunningTask == null || _botRunningTask.IsCompleted)
                {
                    _botRunningTask = new Task(() => RunBotForTags(tags, _cancelationTokenSource.Token));
                    _botRunningTask.Start();
                    _botRunningTask.ContinueWith(task => WebDriverProvider.CloseWebDriver(), TaskScheduler.FromCurrentSynchronizationContext());
                    return _botRunningTask;
                }
                else
                {
                    throw new Exception("cannot start another task because the current one is not completed.");
                }
            }
        }

        public static void RunBotForTags(IEnumerable<Tag> tags, CancellationToken cancellationToken)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            LikesLogger.LogStartOfExecution();

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
            if (tag.Name != "" && int.TryParse(tag.LikesNumber, out likesNumber))
            {
                SearchHashtag(tag.Name);
                LikeSomePosts(likesNumber);
            }
        }

        private static void HandleNotificationsSetting()
        {
            try
            {
                var notNow = WebDriverProvider.WebDriver.FindElement(By.XPath("//button[text()=\"Not Now\"]"));
                notNow.Click();
                ThreadDelayer.WaitSomeTime();
            }
            catch
            {
            }
        }

        private static void SearchHashtag(string tag)
        {
            var hashtag = tag.StartsWith("#") ? tag : "#" + tag;

            var searchBar = WebDriverProvider.WebDriver.FindElement(By.ClassName("XTCLo"));

            searchBar.SendKeys(hashtag);
            ThreadDelayer.WaitSomeTime();

            searchBar.SendKeys(Keys.Return);
            searchBar.SendKeys(Keys.Return);

            ThreadDelayer.WaitSomeTime();
        }

        private static void LikeSomePosts(int postsNumber)
        {
            LoadPostsInPage();
            var popularPostsNumber = 9;
            var likesGiven = popularPostsNumber;
            postsNumber += popularPostsNumber;

            var posts = WebDriverProvider.WebDriver.FindElements(By.ClassName("_9AhH0"));

            while (likesGiven < postsNumber)
            {
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
                    posts = WebDriverProvider.WebDriver.FindElements(By.ClassName("_9AhH0"));
                }
                catch
                {
                    return;
                }
            }
        }

        private static void LoadPostsInPage()
        {
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
            }
        }

        private static void ClickHeart()
        {
            try
            {
                LikesLogger.LogAttempToLike();
                var buttons = WebDriverProvider.WebDriver.FindElements(By.TagName("button"));
                var likeButton = buttons.FirstOrDefault(x => x.FindElements(By.TagName("span")) != null && x.FindElements(By.TagName("span"))
                .Any(y => y.GetAttribute("aria-label") == "Like"));

                if (likeButton != null)
                {
                    likeButton.Click();
                    LikesLogger.LogLikeGiven();
                }
            }
            catch (Exception ex)
            {
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
            catch
            {
            }
        }

        public static void StopRunning()
        {
            _cancelationTokenSource.Cancel();
            LikesLogger.LogStopOfExecution();
            WebDriverProvider.CloseWebDriver();
        }
    }
}
