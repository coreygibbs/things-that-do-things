using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtensions
{
    public static class Extensions
    {

        private static readonly BasicLogger Logger = new BasicLogger();
        /// <summary>
        ///  Extension method to that will also correctly delete cookies in IE
        /// </summary>
        /// <param name="driver"></param>
        public static void DeleteCookies(this IWebDriver driver)
        {
            if (driver == null) return;
            driver.Manage().Cookies.DeleteAllCookies();
            if (driver.GetType() != typeof(InternetExplorerDriver)) return;
            var psInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.SystemDirectory, "RunDll32.exe"),
                Arguments = "InetCpl.cpl,ClearMyTracksByProcess 2",
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var p = new Process { StartInfo = psInfo };
            p.Start();
            p.WaitForExit(60000);
        }

        public static ITakesScreenshot Screenshots(this IWebDriver driver)
        {
            return (ITakesScreenshot)driver;
        }

        /// <summary>
        /// Finds a single element using jquery
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="format"></param>
        /// <param name="argsObjects"></param>
        /// <returns></returns>
        public static RemoteWebElement FindElementByJQuery(this RemoteWebDriver driver, string format, params object[] argsObjects)
        {
            // Return the first item in the collection since we only want one element.  Most likely the collection only contains 1 element anway.
            var elements = FindElementsInternal(driver, String.Format(format, argsObjects));
            if (elements.Count > 0)
            {
                return (RemoteWebElement)FindElementsInternal(driver, String.Format(format, argsObjects))[0];
            }
            return null;
        }

        /// <summary>
        /// Finds a single element using jquery
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="jQuerySelector"></param>
        /// <returns></returns>
        public static RemoteWebElement FindElementByJQuery(this RemoteWebDriver driver, string jQuerySelector)
        {
            // Return the first item in the collection since we only want one element.  Most likely the collection only contains 1 element anway.
            var elements = FindElementsInternal(driver, jQuerySelector);
            if (elements.Count > 0)
            {
                return (RemoteWebElement)elements[0];
                //return (RemoteWebElement)FindElementsInternal(driver, jQuerySelector)[0];
            }
            return null;
        }

        /// <summary>
        /// Finds elements using jquery
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="jQuerySelector"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<RemoteWebElement> FindElementsByJQuery(this RemoteWebDriver driver, string jQuerySelector)
        {
            ReadOnlyCollection<IWebElement> elements = FindElementsInternal(driver, jQuerySelector);
            // The ExecuteScript call made in the FindElementsInternal method only returns a ReadOnlyCollection 
            // with type IWebElement so we need to convert the return elements to RemoteWebElement objects.
            var tempList = new List<RemoteWebElement>();
            foreach (IWebElement item in elements)
            {
                tempList.Add((RemoteWebElement)item);
            }

            return new ReadOnlyCollection<RemoteWebElement>(tempList);
        }

        private static ReadOnlyCollection<IWebElement> FindElementsInternal(RemoteWebDriver driver, string jquerySelector)
        {
            // Check to see if a custom jquery command is coming through.  If not then add the required formatting
            if (jquerySelector.StartsWith("$("))
            {
                jquerySelector = String.Format("return {0}", jquerySelector);
            }
            else
            {
                jquerySelector = String.Format("return $(\"{0}\")", jquerySelector);
            }

            var wait = new DefaultWait<RemoteWebDriver>(driver) { Timeout = TimeSpan.FromSeconds(2) };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            try
            {
                Logger.Debug(String.Format("Executing JQuery: {0}.get()", jquerySelector));
                
                ReadOnlyCollection<IWebElement> collection = wait.Until(e => e.ExecuteScript(jquerySelector + ".get()") as ReadOnlyCollection<IWebElement>);
                return collection;
            }
            catch (Exception)
            {
                Logger.Warning("No elements found with jQuery command: {0}", jquerySelector);
                Logger.Comment("");
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
        }

        /// <summary>
        ///  Performs a hover action on the element
        /// </summary>
        /// <param name="element">IWebElement to hover on</param>
        /// <param name="timeout"></param>
        public static void Hover(this IWebElement element, TimeSpan timeout)
        {
            Logger.TestAction("Hovering on element '{0}'  of type '{1}'", element.Text, element.TagName);
            element.Highlight();
            var wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
                throw new ArgumentException("Element must wrap a web driver", "element");

            IWebDriver driver = wrappedElement.WrappedDriver;

            var builder = new Actions(driver);
            builder.MoveToElement(element).Build().Perform();
            element.WaitForElementToBeVisible(timeout);
            Thread.Sleep(250);
        }

        /// <summary>
        /// Wait for an element to become visible on the page
        /// </summary>
        /// <param name="webElement"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool WaitForElementToBeVisible(this IWebElement webElement, TimeSpan timeout)
        {
            // Call the private method passing in true for the shouldBeVisible variable to indicate the element should be visible
            return WaitForElementVisbility(webElement, timeout, true);
        }

        /// <summary>
        /// Wait for an element to become not visible on the page
        /// </summary>
        /// <param name="webElement"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool WaitForElementToBeNotVisible(this IWebElement webElement, TimeSpan timeout)
        {
            // Call the private method passing in false for the shouldBeVisible variable to indicate the element should be visible
            return WaitForElementVisbility(webElement, timeout, false);
        }

        private static bool WaitForElementVisbility(IWebElement webElement, TimeSpan timeout, bool shouldBeVisible)
        {
            bool result = false;
            try
            {
                var wait = new DefaultWait<IWebElement>(webElement) { Timeout = timeout };
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                wait.IgnoreExceptionTypes(typeof(ArgumentNullException));
                result = wait.Until(e => e.Displayed == shouldBeVisible);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(StaleElementReferenceException))
                {
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        ///     Introduces a wait time for the web driver effectively pausing all actions until the wait time expires
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="waitTime">The amount of time you wish to wait</param>
        public static void Wait(this RemoteWebDriver webDriver, TimeSpan waitTime)
        {
            Thread.Sleep(waitTime);
        }

        /// <summary>
        ///     Clicks on the element and logs information to the test log
        /// </summary>
        /// <param name="element">element to click on</param>
        public static void ClickWithLogging(this RemoteWebElement element)
        {
            Logger.TestAction("Clicking on element '{0}'  of type '<{1}>' at location {2}.", element.Text, element.TagName, element.Location);
            element.Highlight();
            element.Click();
        }

        /// <summary>
        /// Clears the input first and then sends key strokes to the webpage and logs what was sent into the test log
        /// </summary>
        /// <param name="element"></param>
        /// <param name="input"></param>
        public static void SendKeysWithLogging(this RemoteWebElement element, string input)
        {
            Logger.TestAction("Inputing text {0} to element <{1}>.", input, element.TagName);
            element.Clear();
            element.SendKeys(input);
        }

        /// <summary>
        /// Retrieves the text from an input control on the webpage
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string TextFromInput(this RemoteWebElement element)
        {
            return element.GetAttribute("value");
        }

        /// <summary>
        /// Selects a value from a drop down control and logs what was selected into the test log.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        public static void SelectByTextWithLogging(this SelectElement element, string text)
        {
            Logger.TestAction("Selecting dropdown text {0}", text);
            element.SelectByText(text);
        }

        /// <summary>
        /// Selects an element by using text provided and logs the selection
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        public static void SelectByValueWithLogging(this SelectElement element, string text)
        {
            Logger.TestAction("Selecting dropdown value {0}", text);
            element.SelectByValue(text);
        }

        /// <summary>
        /// Selects a value from a drop down control and logs what was selected into the test log.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool Exists(this IWebElement element)
        {
            return element != null;
        }

        /// <summary>
        /// Sets an attribute on an html element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public static void SetAttribute(this IWebElement element, string attributeName, string value)
        {
            var wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
                throw new ArgumentException("Element must wrap a web driver", "element");

            IWebDriver driver = wrappedElement.WrappedDriver;
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution", "element");

            javascript.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
        }

        /// <summary>
        /// Sends keystrokes to the web page element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <param name="clearFirst"></param>
        public static void SendKeys(this IWebElement element, string value, bool clearFirst)
        {
            if (clearFirst) element.Clear();
            element.SendKeys(value);
        }

        /// <summary>
        /// Causes the selected element to flash
        /// </summary>
        /// <param name="element"></param>
        public static void Highlight(this IWebElement element)
        {
            const int wait = 100;
            string orig = element.GetAttribute("style");

            SetAttribute(element, "style", "color: yellow; border: 1px solid yellow; background-color: green;");
            Thread.Sleep(wait);
            SetAttribute(element, "style", "color: green; border: 1px solid black; background-color: yellow;");
            Thread.Sleep(wait);
            SetAttribute(element, "style", "color: yellow; border: 1px solid yellow; background-color: green;");
            Thread.Sleep(wait);
            SetAttribute(element, "style", "color: green; border: 1px solid black; background-color: yellow;");
            Thread.Sleep(wait);
            SetAttribute(element, "style", orig);

        }

        /// <summary>
        /// Waits for the page to load
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeout"></param>
        public static bool WaitForPageToLoad(this IWebDriver driver, TimeSpan timeout)
        {
            var wait = new WebDriverWait(driver, timeout);
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("Driver must support javascript execution", "driver");

            return wait.Until(d =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript(
                        "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Highlights the element being validated
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string BeginValidation(this IWebElement element)
        {
            return element.Highlight("red");
        }

        /// <summary>
        /// Removes the highlight on the elment being validated
        /// </summary>
        /// <param name="element"></param>
        /// <param name="originalStyle"></param>
        public static void EndValidation(this IWebElement element, string originalStyle)
        {
            element.UnHighlight(originalStyle);
        }

        /// <summary>
        /// Removes the highlight on the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="originalStyle"></param>
        public static void UnHighlight(this IWebElement element, string originalStyle)
        {
            SetAttribute(element, "style", originalStyle);
        }

        /// <summary>
        /// Causes the element on the page to be highlighted in the supplied color
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        /// <returns>string of the elements original style</returns>
        public static string Highlight(this IWebElement element, string color)
        {
            string orig = element.GetAttribute("style");
            SetAttribute(element, "style", String.Format("color: black; border: 5px solid {0};", color));
            return orig;
        }

        /// <summary>
        /// Chooses a random elelment from a IEnumerable type such as a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static T Random<T>(this IEnumerable<T> list)
        {
            var rnd = new Random();
            T picked = default(T);
            int cnt = 0;
            foreach (T item in list)
            {
                if (rnd.Next(++cnt) == 0)
                {
                    picked = item;
                }
            }
            return picked;
        }
    }
}