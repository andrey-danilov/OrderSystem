using CsQuery;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OrdersPars;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace DBPars
{
    class Program
    {
        public static void Main()
        {
            Sheduler.Start();
            Console.Read();
        }
    }
}

