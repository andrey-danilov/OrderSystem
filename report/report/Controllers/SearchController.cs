using Newtonsoft.Json;
using NickBuhro.Translit;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace report.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View(new List<string>());
        }

        [HttpPost]
        public ActionResult Index(string sku)
        {
            if (String.IsNullOrEmpty(sku))
            {
                return View(new List<string>());
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ProductPars product = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(sku));
                XmlDocument doc = new XmlDocument();

                string prom = "";
                string promSearch = "";
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://my.prom.ua/api/v1/products/by_external_id/" + sku);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers.Add("Authorization", "Bearer 94c0536c63b1dca0d6dbe44c236fa710e55809bd");


                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string res = "";
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        res = streamReader.ReadToEnd();
                    }
                    dynamic stuff = JsonConvert.DeserializeObject(res);
                    var Id = stuff.product.id;
                    string Name = stuff.product.name;
                    Name = Name.Replace("-", " ").Replace("\"", "");
                    var LStrN = Name.Split(' ').ToList();
                    string CyrName = string.Join("-", LStrN.GetRange(0, 3).ToArray()).ToLower();
                    var latin = Transliteration.CyrillicToLatin(CyrName, Language.Russian);

                    prom = "https://sportbay.com.ua/p" + Id + "-" + latin + ".html";
                    promSearch = "https://sportbay.com.ua/site_search?search_term=" + sku;
                }
                catch
                {
                    promSearch = "https://sportbay.com.ua/site_search?search_term=" + sku;
                }

                return View(new List<string> { product.Url, prom, promSearch });
            }

        }


        [HttpPost]
        public ActionResult Remuve(string sku)
        {
            if (String.IsNullOrEmpty(sku))
            {
                return View(new List<string>() { "Error" });
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    ProductPars product = db.ProductsPars.FirstOrDefault(x => x.VendorCode.Equals(sku));
                    db.ProductsPars.Remove(product);
                    db.SaveChanges();
                    return View(new List<string>() { "Удалено" });
                }
                catch
                {
                    return View(new List<string>() { "SKU не найден/ ошибка при удалении" });
                }
            }
        }
    }
}