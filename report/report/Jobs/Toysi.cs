using Newtonsoft.Json;
using Quartz;
using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace report.Jobs
{

    public class Toysi : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task Execute(IJobExecutionContext context)
        {
            var userId = "aae2ed54-b1c2-496d-b503-64e1b56f4156";

            Konfig CurKonfig = new Konfig();
            WebClient webClient = new WebClient();

            CurKonfig = db.KonfigVe.AsNoTracking().Where(x => x.ShopName.Equals("21Toysi.com.ua") & x.UserId.Equals("aae2ed54-b1c2-496d-b503-64e1b56f4156"))
                    .Include(x => x.AdditionalPictures)
                    .Include(x => x.Denominations)
                    .Include(x => x.Descriptions)
                    .Include(x => x.Excepts)
                    .Include(x => x.Pagination)
                    .Include(x => x.ProductUrl.Availability)
                    .Include(x => x.ProductUrl.CheakPrise)
                    .Include(x => x.PriceLow)
                    .Include(x => x.PriceHigh)
                    .Include(x => x.MainPicture)
                    .Include(x => x.Availability)
                    .Include(x => x.Vendor)
                    .Include(x => x.VendorSearch)
                    .Include(x => x.Contacts)
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();


            CurKonfig.Phone = "";
            CurKonfig.Contacts = new List<Contacts>() { new Contacts() };
            foreach (var SCat in CurKonfig.Categores)
            {

                foreach (var SC in SCat.SubCatecories)
                {

                    foreach (var Ch in SC.Characteristics)
                    {
                        foreach (var Ex in Ch.Exceptions) { }
                    }
                }
            }
            CurKonfig.MainCharacteristics = db.Characteristics.AsNoTracking().Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();

            List<ProductPars> PP = db.ProductsPars.Where(x => x.ShopCode.Equals(CurKonfig.Id)).Include(x => x.Pictures).Include(x => x.Parameters).ToList();

            string PicPath = @"C:\inetpub\wwwroot" + @"\Users\" + userId + @"\" + CurKonfig.ShopName + @"\Photos\";
            List<Models.Picture> Pic = PP.SelectMany(x => x.Pictures).ToList();
            Pic.ForEach(s => s.ProductsPars = null);
            List<Models.Parameters> Par = PP.SelectMany(x => x.Parameters).ToList();
            db.Parameterses.RemoveRange(Par);
            int count = PP.Select(x => Int32.Parse(x.VendorCode.Remove(0, 2))).OrderByDescending(x => x).First();
            var tempPic = new List<Models.Picture>();


            foreach (var p in PP)
            {
                p.Relevance = false;

            }
            db.SaveChanges();

            XmlDocument doc = new XmlDocument();
            doc.Load("https://toysi.ua/feed-products-residue.php?vendor_code=prom&user=1000013537");

            
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("/yml_catalog/shop/offers/offer"))
            {
                try
                {
                    var product = PP.FirstOrDefault(x => x.Url.Equals(node.SelectSingleNode("./url").InnerText));
                    if (product == null) product = new Models.ProductPars();
                    try
                    {
                        product.Denomination = node.SelectSingleNode("./name").InnerText;
                        product.Description = node.SelectSingleNode("./description").FirstChild.InnerText;
                        product.PriceOld = node.SelectSingleNode("./price").InnerText;
                        product.Pictures = new List<Models.Picture>();
                        product.Parameters = new List<Models.Parameters>();
                        product.OurCategori = node.SelectSingleNode("./categoryId").InnerText;

                    }
                    catch
                    {
                        var sds = node.SelectSingleNode("./url").InnerText;
                        sds = "";
                    }
                    string urlMP = node.SelectSingleNode("./picture").InnerText;
                    string format = urlMP.Substring(urlMP.LastIndexOf("."));
                    try
                    {
                        webClient.DownloadFile(urlMP, PicPath + product.VendorCode + format);
                        if (product.Pictures.Count == 0) product.Pictures.Add(new Models.Picture("http://sshop.com.ua/Users/" + userId + "/" + CurKonfig.ShopName + "/Photos/" + product.VendorCode + format));
                    }
                    catch
                    {

                    }

                    if (product == null)
                    {
                        product.Id = Guid.NewGuid().ToString();
                        product.VendorCode = CurKonfig.VendorCode + new string('0', 5 - count.ToString().Length) + count.ToString();
                        PP.Add(product);
                    }
                }
                catch
                {
                    var dfdf = node;
                }
            }
            foreach (var cat in CurKonfig.Categores)
            {
                var Prod = PP.Where(x => x.OurCategori.Equals(cat.url)).ToList();
                foreach (var s in cat.SubCatecories)
                {
                    try
                    {
                        double k = Convert.ToDouble(s.price_processing);
                        foreach (var p in Prod)
                        {
                            p.Relevance = true;
                            try
                            {
                                var a = p.PriceOld.Contains(".") ? p.PriceOld.Remove(p.PriceOld.IndexOf(".")) : p.PriceOld;
                                double vull = 100.0;
                                p.Price = Convert.ToString(Convert.ToInt32(Convert.ToDouble(a) + ((Convert.ToDouble(a) / vull) * k)));
                            }
                            catch
                            {
                                var a = p.PriceOld;
                                var dfd = k;
                            }
                            p.OurCategori = s.NameOurCategory;
                            p.PortalCategoryId = s.PortalCategoryId;
                        }
                        foreach (var par in s.Characteristics)
                        {
                            var NewPar = new Models.Parameters(par.Name, par.Value, false);
                            NewPar.Id = Guid.NewGuid().ToString();
                            NewPar.ProductsPars = Prod;
                            db.Parameterses.Add(NewPar);
                        }
                    }
                    catch
                    {
                        double k = Convert.ToDouble(s.price_processing);
                    }

                }
            }
            db.SaveChanges();
        }
    }
}
