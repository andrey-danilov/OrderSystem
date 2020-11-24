using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using report.Controllers;

namespace reportUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            OrderController OC = new OrderController();
            string a = "";
            OC.Details(a);
                

                
        }
    }
}
