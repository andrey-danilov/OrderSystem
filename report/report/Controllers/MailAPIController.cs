using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace report.Controllers
{
    public class MailAPIController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: MailAPI
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string DocumentNumber)
        {
            string result = Send(DocumentNumber, "");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            string PhoneSender = "";
            if (doc.DocumentElement.SelectSingleNode("/root/data/item") != null)
            {
                try
                {
                    PhoneSender = doc.DocumentElement.SelectSingleNode("/root/data/item/PhoneSender").InnerText;
                }
                catch
                {
                    return PartialView("request", "Не удалось найти номер отправщика");
                }
            }

            result = Send(DocumentNumber, PhoneSender);
            doc.LoadXml(result);
            string RecipientFullName = "";
            if (doc.DocumentElement.SelectSingleNode("/root/data/item") != null)
            {
                try
                {
                    RecipientFullName = doc.DocumentElement.SelectSingleNode("/root/data/item/RecipientFullNameEW").InnerText;
                }
                catch
                {
                    RecipientFullName = doc.DocumentElement.SelectSingleNode("/root/data/item/RecipientFullName").InnerText;
                }
            }
            RecipientFullName = RecipientFullName.Remove(RecipientFullName.IndexOf(" "));
            try
            {
                OrderProm a = db.OP.Where(x => x.DeliveryInfo.Contains(RecipientFullName.TrimEnd())).Include(x=> x.Statuses).Include(x=>x.Products).Include(x=> x.Notes).OrderBy(x=> x.number).FirstOrDefault();
                
                if (!String.IsNullOrEmpty(a.Id))
                {
                    try
                    {
                        a.Statuses.FirstOrDefault(x => x.StatusName.Equals("ТТН")).flag = true;
                    }
                    catch
                    {
                    }
                    a.WaybillNumber = DocumentNumber;
                    db.SaveChanges();
                }
                return PartialView("Save_changes", a);
            }
            catch
            {
                return PartialView("request", "Не удалось найти Ф.И.О покупателя");
            }
            
            
        }

        public string Send(string DocumentNumber, string Phone)
        {
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.novaposhta.ua/v2.0/xml/");
            httpWebRequest.ContentType = "text/xml;";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string xml = String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>{0}<root>{0}", Environment.NewLine) +
                                String.Format("<apiKey>{0}</apiKey>{1}", "cb2c4c0c7a9fd02dd40569f497a59587", Environment.NewLine) +
                                String.Format("<calledMethod>getStatusDocuments</calledMethod>{0}<methodProperties>{0}<Documents>{0}<item>{0}", Environment.NewLine) +
                                String.Format("<DocumentNumber>{0}</DocumentNumber>{2}<Phone>{1}</Phone>{2}</item>{2}", DocumentNumber, (Phone.Equals(""))? "380963430001" : Phone, Environment.NewLine) +
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
            return result;
        }
       

    }
}
