using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using TestSamples.PageObjects;
using SeleniumExtensions;
using OpenQA.Selenium.Remote;
using System.Drawing.Imaging;
using TestSamples.Interfaces;

namespace Ui.Tests.ScenarioTests
{
    [TestClass]
    public class SampleTests
    {
        private  RemoteWebDriver _webDriver;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _webDriver = new ChromeDriver();
            // Set time the framework should wait for elements to show up in the DOM
            _webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            _webDriver.Manage().Window.Maximize();
            // Go here first so we can clear cookies correctly.
            _webDriver.Navigate().GoToUrl("");
            _webDriver.DeleteCookies();
            // Move the mouse to the corner of the screen so the focus won't interfere with the test.
            WinAPI.MouseMove(0, 0);
        }

        [TestCleanup]
        public virtual void TestTearDown()
        {
            // Only take a screenshot if the test failed.
            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                _webDriver.Screenshots().GetScreenshot().SaveAsFile(String.Format("{0}_screenCap.jpeg", TestContext.TestName), ImageFormat.Jpeg);
            }

            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [TestMethod]
        [TestCategory("Default")]
        public void SearchAutocomplete()
        {
            var msnHomePage = new POHomePage(_webDriver);
            GoToPage("http://www.msn.com", msnHomePage);

            msnHomePage.SearchTermInputbox.SendKeysWithLogging("Selenium");

            // Validate autocomplete dropdown
            Assert.IsTrue(msnHomePage.TopHitsAutoCompleteBlock.WaitForElementToBeVisible(TimeSpan.FromSeconds(5)),
                "Auto complete block did not show up as expected.");
        }

        [TestMethod]
        [TestCategory("Default")]
        public void SlideShowNavigation()
        {
            var msnHomePage = new POHomePage(_webDriver);
            GoToPage("http://www.msn.com", msnHomePage);
            // Click a nav button once which will stop the auto nav
            msnHomePage.SlideShow.GoRightButton.ClickWithLogging();
            // Get the href for the current slide
            var currentHref = msnHomePage.SlideShow.GetSelectedSlideLink.GetAttribute("href");

            msnHomePage.SlideShow.GoRightButton.ClickWithLogging();
            _webDriver.Wait(TimeSpan.FromSeconds(1));

            // Get the href of the current slide
            var newCurrentHref = msnHomePage.SlideShow.GetSelectedSlideLink.GetAttribute("href");

            // Now check to make sure they are not the same
            Assert.AreNotEqual(currentHref, newCurrentHref, 
                "Href values are the same for 2 slides.  The slide show may not have advanced.");

        }

        [TestMethod]
        [TestCategory("Default")]
        public void WeatherSectionNavigation()
        {
            _webDriver.Manage().Window.Size = new System.Drawing.Size(800, 800);

            var msnHomePage = new POHomePage(_webDriver);
            GoToPage("http://www.msn.com", msnHomePage);
            if (msnHomePage.PromoBannerCloseButton.Displayed)
            {
                msnHomePage.PromoBannerCloseButton.ClickWithLogging();
            }
            // Wait for the banner ad to not steal focus
            _webDriver.Wait(TimeSpan.FromSeconds(2));

            // Used to make the page scroll to ensure elment is visible.
            msnHomePage.EntertainmentSection.RightArrowButton.Focus();
            _webDriver.Wait(TimeSpan.FromSeconds(1));
            
            // Wait time mainly here so you can visibily see that things work.
            msnHomePage.NewsSection.RightArrowButton.ClickWithLogging();
            _webDriver.Wait(TimeSpan.FromSeconds(1));
            
            msnHomePage.NewsSection.LeftArrowButton.ClickWithLogging();
            _webDriver.Wait(TimeSpan.FromSeconds(1));
        }

        private void GoToPage(string url, IPageObject page)
        {
            _webDriver.Navigate().GoToUrl(url);
            var outcome = page.IsLoaded(TimeSpan.FromSeconds(10));
            Assert.IsTrue(outcome.Passed, outcome.ToString());
        }

    }

}