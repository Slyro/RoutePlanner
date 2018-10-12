using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RoutePlanner
{
    enum PropertyType
    {
        Id,
        Name,
        CssSelector,
        XPath,
        LinkText,
        ClassName,
        PartialLinkText
    }
    class PropertiesCollections
    {
        public static IWebDriver driver { get; set; }
        public static ChromeDriverService service { get; set; }
        public static IJavaScriptExecutor javaScriptExecutor { get; set; }
    }
}
