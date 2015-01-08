using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtensions
{
    public class DropDownElement
    {
        public DropDownElement(IWebElement webElement)
        {
            Element = webElement;
            Option = new SelectElement(webElement);
        }

        public IWebElement Element { get; private set; }
        public SelectElement Option { get; private set; }
    }
}