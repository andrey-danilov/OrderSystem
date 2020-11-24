using Newtonsoft.Json;
using Quartz;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace report.Jobs
{
    public class OrderPars : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            var ListEx = new List<PromExport>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ListEx = db.PromExports.ToList();
            }

            foreach(var ex in ListEx)
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(ex.Url);
                List<OrderProm> orders = new List<OrderProm>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    foreach (XmlNode Order in doc.SelectNodes("/orders/order"))
                    {
                        string Id = Order.Attributes["id"].InnerText;
                        if (db.OP.FirstOrDefault(x => x.Order_Number.Equals(Id)) != null)
                        {
                            break;
                        }
                        else
                        {
                            string Num = Order.Attributes["id"].InnerText;
                            string BuyersName = "";
                            string Phone = "";
                            string Data = "";
                            string TotalPrice = "";
                            string DeliveryInfo = "";
                            try
                            {
                                BuyersName = Order.SelectSingleNode("./name").InnerText;
                            }
                            catch { }
                            try
                            {
                                Phone = Order.SelectSingleNode("./phone").InnerText;
                            }
                            catch { }
                            try
                            {
                                Data = Order.SelectSingleNode("./date").InnerText;
                            }
                            catch { }
                            try
                            {
                                TotalPrice = Order.SelectSingleNode("./priceUAH").InnerText;
                                if (TotalPrice.IndexOf(".") > -1) TotalPrice = TotalPrice.Remove(TotalPrice.IndexOf("."));
                            }
                            catch { }
                            try
                            {
                                DeliveryInfo += Order.SelectSingleNode("./name").InnerText;
                                DeliveryInfo += Order.SelectSingleNode("./address").InnerText;
                                DeliveryInfo = Regex.Replace(DeliveryInfo, @"\s+", " ");
                            }
                            catch { }
                            List<UserStatus> US = db.UserStatuses.AsNoTracking().ToList();
                            short Ordernum = (short)db.OP.ToList().Count;
                            OrderProm temp = new OrderProm("aae2ed54-b1c2-496d-b503-64e1b56f4156", Num, Phone, Data, TotalPrice, DeliveryInfo, Ordernum);
                            temp.Statuses = new List<Status>();

                            foreach (UserStatus t in US)
                            {
                                temp.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                            }
                            temp.Statuses.FirstOrDefault(x => x.StatusName.Contains("Контроль")).flag = true;
                            try
                            {
                                var paymentType = Order.SelectSingleNode("./paymentType").InnerText;
                                if(paymentType.Contains("Наложенный платеж")) temp.Statuses.FirstOrDefault(x => x.StatusName.Contains("Наложенный")).flag = true;
                            }
                            catch { }
                            temp.Products = new List<ProductProm>();
                            temp.Notes = new List<Note>();


                            foreach (XmlNode nod in Order.SelectNodes("./labels/label"))
                            {
                                short count = 0;
                                if (!nod.InnerText.Equals("контроль"))
                                {
                                    temp.Notes.Add(new Note() { NoteName = nod.InnerText, flag = false, number = count });
                                    count++;
                                }
                            }
                            try
                            {
                                short count = 0;
                                temp.Notes.Add(new Note() { NoteName = Order.SelectSingleNode("./payercomment").InnerText, flag = false, number = count });
                            }
                            catch { }

                            foreach (XmlNode item in Order.SelectNodes("./items/item"))
                            {
                                string VendorCode = item.SelectSingleNode("./external_id").InnerText;
                                string Price = item.SelectSingleNode("./price").InnerText;
                                if (Price.IndexOf(".") > -1) Price = Price.Remove(Price.IndexOf("."));
                                string Quantity = item.SelectSingleNode("./quantity").InnerText;
                                if (Quantity.IndexOf(".") > -1) Quantity = Quantity.Remove(Quantity.IndexOf("."));
                                string Url = item.SelectSingleNode("./url").InnerText;
                                string Name = item.SelectSingleNode("./name").InnerText;
                                string TVC = VendorCode;
                                try
                                {
                                    if (VendorCode.Contains("-"))
                                    {
                                        TVC = VendorCode.Remove(VendorCode.IndexOf("-"));
                                    }
                                    string VU = "";
                                    try
                                    {
                                        VU = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(TVC)).Url;
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            VU = db.ProductsPars.FirstOrDefault(x => x.Denomination.Equals(Name)).Url;
                                        }
                                        catch
                                        {
                                            string UR = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(TVC.Remove(2))).Url;
                                            int i = 1;
                                            for (int j = 0; j < UR.Count(); j++)
                                            {
                                                if (UR[j].Equals('/'))
                                                {
                                                    i++;
                                                }
                                                if (i == 3)
                                                {
                                                    VU = UR.Remove(j);
                                                }

                                            }
                                        }

                                    }
                                    ProductProm tempP = new ProductProm(VendorCode, VU, Price, Quantity, Url, Name);
                                    temp.Products.Add(tempP);
                                }
                                catch
                                {
                                    ProductProm tempP = new ProductProm(VendorCode, Url, Price, Quantity, Url, Name);
                                    temp.Products.Add(tempP);
                                }
                                
                                
                                
                            }
                            orders.Add(temp);
                        }
                    }
                    orders.Reverse();
                    List<string> OrderNum = new List<string>() {};
                    foreach (OrderProm order in orders)
                    {
                        OrderNum.Add(order.Order_Number);
                        short c = (short)db.OP.ToList().Count;
                        c++;
                        order.number = c;
                        db.OP.Add(order);
                        db.SaveChanges();
                    }
                    if(OrderNum.Count>0)
                    {
                    

                        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://my.prom.ua/api/v1/orders/set_status");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Headers.Add("Authorization", "Bearer 94c0536c63b1dca0d6dbe44c236fa710e55809bd");
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = "";
                            if (OrderNum.Count == 1)
                            {
                                json = "{ \"ids\": [" +OrderNum.First()+"]  ,  \"status\": \"received\"}";
                            }
                            else
                            {
                                json = "{ \"ids\": [" + string.Join(", ", OrderNum.ToArray()) + "]  ,  \"status\": \"received\"}";
                            }
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                        }
                    }
                }

            }

            XmlDocument docFex = new XmlDocument();
            docFex.Load("https://fex.com.ua/components/com_jshopping/files/importexport/exportorders/exportorders_816a2e5062af62c40bb78a58a6f10e3b13bcb7ff44b175141e4862614774d7a0.xml");
            List<OrderProm> ordersForFex = new List<OrderProm>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (XmlNode Order in docFex.SelectNodes("/orders/order"))
                {
                    string Id = Order.Attributes["id"].InnerText;
                    if (db.OP.FirstOrDefault(x => x.Order_Number.Equals(Id)) != null)
                    {
                        break;
                    }
                    else
                    {
                        string Num = Order.Attributes["id"].InnerText;
                        string status = Order.Attributes["state"].InnerText;
                        string BuyersName = "";
                        string Phone = "";
                        string Data = "";
                        string TotalPrice = "";
                        string DeliveryInfo = "";
                        try
                        {
                            BuyersName = Order.SelectSingleNode("./name").InnerText;
                        }
                        catch { }
                        try
                        {
                            Phone = Order.SelectSingleNode("./phone").InnerText;
                        }
                        catch { }
                        try
                        {
                            Data = Order.SelectSingleNode("./date").InnerText;
                        }
                        catch { }
                        try
                        {
                            TotalPrice = Order.SelectSingleNode("./priceUAH").InnerText;
                            if (TotalPrice.IndexOf(".") > -1) TotalPrice = TotalPrice.Remove(TotalPrice.IndexOf("."));
                        }
                        catch { }
                        try
                        {
                            DeliveryInfo += Order.SelectSingleNode("./name").InnerText;
                            DeliveryInfo += Order.SelectSingleNode("./address").InnerText;
                            DeliveryInfo = Regex.Replace(DeliveryInfo, @"\s+", " ");
                        }
                        catch { }
                        List<UserStatus> US = db.UserStatuses.AsNoTracking().ToList();
                        short Ordernum = (short)db.OP.ToList().Count;
                        OrderProm temp = new OrderProm("aae2ed54-b1c2-496d-b503-64e1b56f4156", Num, Phone, Data, TotalPrice, DeliveryInfo, Ordernum);
                        temp.Statuses = new List<Status>();

                        foreach (UserStatus t in US)
                        {
                            temp.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                        }
                        temp.Statuses.FirstOrDefault(x => x.StatusName.Contains("Контроль")).flag = true;
                        try
                        {
                            var paymentType = Order.SelectSingleNode("./paymentType").InnerText;
                            if (paymentType.Contains("Наложенный платеж")) temp.Statuses.FirstOrDefault(x => x.StatusName.Contains("Наложенный")).flag = true;
                            if (paymentType.Contains("WayForPay")) temp.Statuses.FirstOrDefault(x => x.StatusName.Contains("Оплачен")).flag = true;
                        }
                        catch { }
                        temp.Products = new List<ProductProm>();
                        temp.Notes = new List<Note>();


                        foreach (XmlNode nod in Order.SelectNodes("./labels/label"))
                        {
                            short count = 0;
                            if (!nod.InnerText.Equals("контроль"))
                            {
                                temp.Notes.Add(new Note() { NoteName = nod.InnerText, flag = false, number = count });
                                count++;
                            }
                        }
                        try
                        {
                            short count = 0;
                            temp.Notes.Add(new Note() { NoteName = Order.SelectSingleNode("./source").InnerText, flag = false, number = count });
                        }
                        catch { }

                        foreach (XmlNode item in Order.SelectNodes("./items/item"))
                        {
                            string VendorCode = item.SelectSingleNode("./external_id").InnerText;
                            string Price = item.SelectSingleNode("./price").InnerText;
                            if (Price.IndexOf(".") > -1) Price = Price.Remove(Price.IndexOf("."));
                            string Quantity = item.SelectSingleNode("./quantity").InnerText;
                            if (Quantity.IndexOf(".") > -1) Quantity = Quantity.Remove(Quantity.IndexOf("."));
                            string Url = item.SelectSingleNode("./url").InnerText;
                            string Name = item.SelectSingleNode("./name").InnerText;
                            string TVC = VendorCode;
                            try
                            {
                                if (VendorCode.Contains("-"))
                                {
                                    TVC = VendorCode.Remove(VendorCode.IndexOf("-"));
                                }
                                string VU = "";
                                try
                                {
                                    VU = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(TVC)).Url;
                                }
                                catch
                                {
                                    try
                                    {
                                        VU = db.ProductsPars.FirstOrDefault(x => x.Denomination.Equals(Name)).Url;
                                    }
                                    catch
                                    {
                                        string UR = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(TVC.Remove(2))).Url;
                                        int i = 1;
                                        for (int j = 0; j < UR.Count(); j++)
                                        {
                                            if (UR[j].Equals('/'))
                                            {
                                                i++;
                                            }
                                            if (i == 3)
                                            {
                                                VU = UR.Remove(j);
                                            }

                                        }
                                    }

                                }
                                ProductProm tempP = new ProductProm(VendorCode, VU, Price, Quantity, Url, Name);
                                temp.Products.Add(tempP);
                            }
                            catch
                            {
                                ProductProm tempP = new ProductProm(VendorCode, Url, Price, Quantity, Url, Name);
                                temp.Products.Add(tempP);
                            }
                        }
                        ordersForFex.Add(temp);
                    }
                }
                ordersForFex.Reverse();
                List<string> OrderNum = new List<string>() { };
                foreach (OrderProm order in ordersForFex)
                {
                    OrderNum.Add(order.Order_Number);
                    short c = (short)db.OP.ToList().Count;
                    c++;
                    order.number = c;
                    db.OP.Add(order);
                    db.SaveChanges();
                }
            }


        }
    }
}