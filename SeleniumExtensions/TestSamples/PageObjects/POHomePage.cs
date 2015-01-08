using OpenQA.Selenium.Remote;
using System;
using TestSamples.Interfaces;
using TestSamples.Objects;
using SeleniumExtensions;

namespace TestSamples.PageObjects
{
    public class POHomePage : IPageObject
    {
        private readonly RemoteWebDriver webDriver;

        public POHomePage(RemoteWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        public ValidationResult IsLoaded(TimeSpan timeout)
        {
            var result = new ValidationResult();
            if (webDriver.WaitForPageToLoad(timeout)) 
            {
                return result; 
            }

            result.ErrorCount++;
            result.ErrorMessages.Add("Required control not found on the page.  It may not have loaded correctly.");
            return result;
            
        }

        public ValidationResult ValidateDefaultRenderState()
        {
            throw new NotImplementedException();
        }

        public RemoteWebElement SearchTermInputbox
        {
            get { return webDriver.FindElementByJQuery("div.qc input"); }
        }

        public RemoteWebElement SearchButton
        {
            get { return webDriver.FindElementByJQuery("button#sb_form_go"); }
        }

        public RemoteWebElement TopHitsAutoCompleteBlock
        {
            get { return webDriver.FindElementByJQuery("div.sa_as.sa_se"); }
        }

        public HomePageSlideShowControl SlideShow
        {
            get { return new HomePageSlideShowControl(webDriver); }
        }
    }

    public class HomePageSlideShowControl : IPageObject
    {
        private RemoteWebDriver webDriver;

        public HomePageSlideShowControl(RemoteWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        public ValidationResult IsLoaded(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public ValidationResult ValidateDefaultRenderState()
        {
            throw new NotImplementedException();
        }

        public RemoteWebElement MainWindow
        {
            get { return webDriver.FindElementByJQuery("div.ip.infopanestripe.slideshow"); }
        }

        public RemoteWebElement GoRightButton
        {
            get { return webDriver.FindElementByJQuery("div.ip.infopanestripe.slideshow button.rightarrow"); }
        }

        public RemoteWebElement GoLeftButton
        {
            get { return webDriver.FindElementByJQuery("div.ip.infopanestripe.slideshow button.leftarrow"); }
        }

        public RemoteWebElement GetSelectedSlideLink
        {
            get { return webDriver.FindElementByJQuery("div.ip.infopanestripe.slideshow li.selected a"); }
        }

    }
}