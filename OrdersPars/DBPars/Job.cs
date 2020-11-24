using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrdersPars
{
    class Job : IJob 
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var date = DateTime.Now;
            
            
            string Url_login = "https://my.prom.ua/cabinet/sign-in";
            string Xpath_Login = "//*[@id=\"phone_email\"]";
            string Xpath_Password = "//*[@id=\"password\"]";
            var options = new ChromeOptions();
            options.AddArgument("headless");
            IWebDriver PJSDriver = new ChromeDriver(@"C:\pjs");
            string Xpath_Page = "/html/body/div[6]/div[7]/div[2]/div[2]/a[2]";
            Thread.Sleep(1000);
            PJSDriver.Navigate().GoToUrl(Url_login);
            Thread.Sleep(2000);
            IWebElement L = PJSDriver.FindElement(By.Id("phone_email"));
            L.SendKeys(@"tennisballs@ya.ru");
            Thread.Sleep(1000);
            IWebElement P = PJSDriver.FindElement(By.Id("password"));
            P.SendKeys("tsshat76");
            Thread.Sleep(2000);
            IWebElement SingIn = PJSDriver.FindElement(By.Id("submit_login_button"));
            SingIn.Click();
            PJSDriver.Navigate().GoToUrl("https://my.prom.ua/cms/prosale");
            Thread.Sleep(1000);

            var AllProd = PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label > span"))[0].Text;
            var Bigl = PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label > span"))[1].Text;



            if (AllProd.Equals("Запущена") & date.Hour < 1 & date.Hour >= 0)
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[0].Click();
            }
            else if (AllProd.Equals("Запустить") & date.Hour >= 9)
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[0].Click();
            }
            else if (AllProd.Equals("Запущена") & date.Hour == 20)
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[0].Click();
            }
            
            
            if (Bigl.Equals("Запущена") & date.Hour < 1 & date.Hour >= 0 )
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[1].Click();
            }
            else if (Bigl.Equals("Запустить") & date.Hour >= 9)
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[1].Click();
            }
            else if (Bigl.Equals("Запущена") & date.Hour == 20)
            {
                PJSDriver.FindElements(By.CssSelector("div.b-prosale-item__tumbler-cell > label"))[1].Click();
            }
            Thread.Sleep(1000);

            PJSDriver.Close();
            PJSDriver.Quit();
        }
    }
}
