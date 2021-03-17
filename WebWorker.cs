using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace mJinny
{
    class WebWorkThread
    {
        const string gmURL = "https://gmail.com/";

        public const string ProfileDirPrefix = "Profile";
        public const string AnyProfileParentDir = "Default";

        public string ChromeDataDir = "";
        public Thread thread;
        public int ThreadID;
        public string LastErrorMsg;
        private SyncLog log;

        private FirefoxDriverService service;
        private FirefoxDriver driver;

        public WebWorkThread(int threadID, SyncLog logger)
        {
            log = logger;
            ThreadID = threadID;

            thread = new Thread(WebTaskProcess)
            {
                Name = "WWThread" + ThreadID,
                IsBackground = true
            };
        }
        ~WebWorkThread()
        {
            CloseDriver();
        }
        public void Start()
        {
            if (null != thread)
            {
                thread.Start();
            }
        }

        // Main work thread
        void WebTaskProcess()
        {
            this.service = FirefoxDriverService.CreateDefaultService();
            //service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            FirefoxOptions opt = new FirefoxOptions();
            //opt.AddArgument("--user-data-dir=" + ChromeDataDir);

            CreateDriver(opt, out driver);

            Login("sergiy.shishko@gmail.com", "hardstuff!69"); // TODO

            CloseDriver();
        }

        void CreateDriver(FirefoxOptions opt, out FirefoxDriver driver)
        {
            opt.SetLoggingPreference(LogType.Driver, LogLevel.Off); // TODO
            opt.SetLoggingPreference(LogType.Browser, LogLevel.Off);// TODO

            driver = new FirefoxDriver(service, opt, TimeSpan.FromMinutes(5));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);

            log.Write( "driver has been created");
        }

        private void CloseDriver()
        {
            try
            {
                driver.Close();
                driver.Quit();
                driver = null;
            }
            catch { }
            driver = null;
        }

        void Navigate(string url/*, string referrer*/)
        {
            // Save the current page element
            var currElem = driver.FindElementByTagName("html");

            // Go Gmail:
            driver.Navigate().GoToUrl(url);

            int waitTimeout = 60000;
            int waitTotal = 0;
            int waitStep = 2000;
            bool reached = driver.FindElementByTagName("html") != currElem;
            while (!reached && waitTotal < waitTimeout)
            {
                Thread.Sleep(waitStep);
                waitTotal += waitStep;

                reached = driver.FindElementByTagName("html") != currElem;
            }

        }

        void Login(string login, string password)
        {
            Navigate(gmURL);

            Actions action = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            //TODO create a function to add "@gmail.com" if absent
            IWebElement loginInput = driver.FindElement(By.Id("identifierId"));
            action.MoveToElement(loginInput).Build().Perform();
            loginInput.SendKeys(login);

            //IWebElement btnNext = driver.FindElement(By.Name("identifierNext"));
            //driver.FindElement(By.Name("identifierNext")).Click();
            driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[2]/div/div[1]/div/div/button/div[2]")).Click();

            // action.MoveToElement(btnNext).Build().Perform();
            //btnNext.Click();

            //wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='img-defer-id-1-6775']")));
            //action.MoveToElement(driver.FindElement(By.XPath("//*[@id='img-defer-id-1-6775']"))).Build().Perform();
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.XP ath("//*[@id='account-sub-nav']/div/div[2]/ul/li[1]/div/span/span[3]/a"))).Click();
            Console.ReadKey();
            Console.WriteLine("Hooray!");
        }

    } 

}