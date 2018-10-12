//using OpenQA.Selenium.Support.PageObjects;
//using OpenQA.Selenium;
//using System.Threading;

//namespace RoutePlanner
//{
//   class AKDPageObjects
//    {
//        public AKDPageObjects()
//        {
//            PageFactory.InitElements(PropertiesCollections.driver, this);
//        }
//        [FindsBy(How = How.Name, Using = "login")]
//        public IWebElement LoginTextField { get; set; }

//        [FindsBy(How = How.Name, Using = "password")]
//        public IWebElement PasswordTextField { get; set; }

//        [FindsBy(How = How.Id, Using = "mx.app.page.Login.enter")]
//        public IWebElement EnterButton { get; set; }

//        public void Login(string username, string password)
//        {
//            while (!AKDTools.IsReady)
//            {

//            }
//            Thread.Sleep(2000);// Адовый костыль. :(
//            LoginTextField.SendKeys(username);
//            PasswordTextField.SendKeys(password);
//            EnterButton.Click();
//        }
//    }
//}
