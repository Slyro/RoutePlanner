using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace RoutePlanner
{
    class DriverManager
    {
        public static string CurrentWindowHande => PropertiesCollections.driver.CurrentWindowHandle;
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> WindowHandles => PropertiesCollections.driver.WindowHandles;
        public static string Url
        {
            get => PropertiesCollections.driver.Url;
            set => PropertiesCollections.driver.Url = value;
        }
        public static void CreateDriverAndOpenUrl(string site_url, TimeSpan implicitWait)
        {
            StartDriver();
            ImplicitlyWait(implicitWait);
            GoToUrl(site_url);
        }
        public static void CreateDriverAndOpenUrl(string site_url)
        {
            StartDriver();
            ImplicitlyWait(TimeSpan.FromSeconds(10));
            GoToUrl(site_url);
        }
        public static void NewTab() => ExecuteScript("windows.open();");
        public static void NewTab(string url) => ExecuteScript("windows.open(" + url + ");");
        public static IWebElement FindElementById(string id) => PropertiesCollections.driver.FindElement(By.Id(id));
        public static IWebElement FindElementByName(string name) => PropertiesCollections.driver.FindElement(By.Name(name));
        private static void StartDriver()
        {
            PropertiesCollections.service = ChromeDriverService.CreateDefaultService();
            PropertiesCollections.service.HideCommandPromptWindow = true;
            PropertiesCollections.driver = new ChromeDriver(PropertiesCollections.service);
            PropertiesCollections.javaScriptExecutor = PropertiesCollections.driver as IJavaScriptExecutor;  
        }
        private static void ImplicitlyWait(TimeSpan waitTime) => PropertiesCollections.driver.Manage().Timeouts().ImplicitWait = waitTime;
        private static void GoToUrl(string url) => PropertiesCollections.driver.Navigate().GoToUrl(url);
        public static void Back() => PropertiesCollections.driver.Navigate().Back();
        public static object ExecuteScript(string script, params object[] args) => PropertiesCollections.javaScriptExecutor.ExecuteScript(script, args);
        public static void Quit()
        {
            if (PropertiesCollections.driver != null)
                PropertiesCollections.driver.Quit();
        }
        public static bool IsRunning() => PropertiesCollections.driver != null ? true : false;
    }
}