using Quartz;
using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace report.Jobs
{
    public class Tracking : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task Execute(IJobExecutionContext context)
        {
            List<OrderProm> Orders = db.OP.Where(x => !String.IsNullOrEmpty(x.WaybillNumber)).Include(x=> x.Statuses).ToList();
            Orders = Orders.Where(x => x.Statuses.FirstOrDefault(y => y.StatusName.Contains("Контроль") & y.flag) != null).ToList();
            List<List<OrderProm>> LLOP = new List<List<OrderProm>>();

            while(Orders.Count>0)
            {
                LLOP.Add(Orders.Take(100).ToList());
                Orders.RemoveRange(0, LLOP.Last().Count);
            }
            foreach(List<OrderProm> OP in LLOP)
            {
                string Items = "";

                foreach (OrderProm order in OP)
                {
                    Items += String.Format("<item>{2}<DocumentNumber>{0}</DocumentNumber>{2}<Phone>{1}</Phone>{2}</item>{2}", order.WaybillNumber, "380963430001", Environment.NewLine);
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.novaposhta.ua/v2.0/xml/");
                httpWebRequest.ContentType = "text/xml;";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string xml = String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>{0}<root>{0}", Environment.NewLine) +
                                    String.Format("<apiKey>{0}</apiKey>{1}", "cb2c4c0c7a9fd02dd40569f497a59587", Environment.NewLine) +
                                    String.Format("<calledMethod>getStatusDocuments</calledMethod>{0}<methodProperties>{0}<Documents>{0}{1}", Environment.NewLine, Items) +
                                    String.Format("</Documents>{0}</methodProperties>{0}<modelName>TrackingDocument</modelName>{0}</root>", Environment.NewLine);

                    streamWriter.Write(xml);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                string result = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);

                if (doc.DocumentElement.SelectSingleNode("/root/data") != null)
                {
                    foreach (XmlNode a in doc.DocumentElement.SelectNodes("/root/data/item"))
                    {
                        try
                        {
                            OrderProm Temp = OP.FirstOrDefault(x => x.WaybillNumber.Equals(a.SelectSingleNode("./Number").InnerText));
                            if(Temp.FromShop)
                            {
                                string url = String.Format("http://localhost:57423/Orders/AddTrackingStatus?id={0}&TrackingStatus={1}", Temp.Id, a.SelectSingleNode("./Status").InnerText);
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                request.Method = "GET";
                                request.Accept = "application/json";
                                request.UserAgent = "Mozilla/5.0 ....";

                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                StreamReader reader = new StreamReader(response.GetResponseStream());
                                StringBuilder output = new StringBuilder();
                                output.Append(reader.ReadToEnd());
                                response.Close();
                            }                            
                            Thread.Sleep(1);
                            Temp.Traking = a.SelectSingleNode("./Status").InnerText;
                            Thread.Sleep(1);
                            Temp.TrakingStatus = Convert.ToInt32(a.SelectSingleNode("./StatusCode").InnerText);
                            Thread.Sleep(1);
                            db.SaveChanges();

                        }
                        catch
                        {
                            Thread.Sleep(1);
                        }

                    }
                }
            }

        }


    }
}