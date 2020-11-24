using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using report.Models;
using System.Xml;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Threading;
using System.Diagnostics;
using OfficeOpenXml;
using CsQuery;
using System.Net;

namespace report.Controllers
{
    public class ShopsController : Controller
    {
        // GET: Shops
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + "/Users/" + userId);

            List<string> shops = new List<string>();
            foreach (DirectoryInfo p in user.GetDirectories().ToList())
            {
                shops.Add(p.Name);
            }
            //XmlDocument doc = new XmlDocument();
            //doc.Load(user.FullName + @"\yandex_market.xml");
            //Dictionary<string, string> NameSKU = new Dictionary<string, string>();
            //foreach (XmlNode MO in doc.SelectNodes("offers/offer"))
            //{
            //    string name = MO.SelectSingleNode(".//name").InnerText;
            //    string Sku = MO.SelectSingleNode(".//vendorCode").InnerText;

            //    try
            //    {
            //        NameSKU.Add(name, Sku);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}
            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    foreach (Konfig K in db.KonfigVe.ToList())
            //    {
            //        List<ProductPars> prod = db.ProductsPars.Where(x => x.ShopCode.Equals(K.Id)).Include(x => x.Parameters).Include(x => x.Pictures).ToList();
            //        List<string> prodUpID = new List<string>();
            //        if (prod.Count > 0)
            //        {
            //            foreach (ProductPars PP in prod)
            //            {
            //                KeyValuePair<string, string> a = NameSKU.FirstOrDefault(x => x.Key.Equals(PP.Denomination));
            //                if (a.Key != null)
            //                {
            //                    if (K.VendorCode.Equals(a.Value.Remove(2)))
            //                    {
            //                        PP.VendorCode = a.Value;
            //                        prodUpID.Add(PP.Id);
            //                    }

            //                }
            //            }
            //            int i = 1;
            //            foreach (ProductPars PP in prod)
            //            {
            //                if (prodUpID.Count == 0) break;
            //                if (prodUpID.FirstOrDefault(x => x.Equals(PP.Id)) != null)
            //                {
            //                    string temp = K.VendorCode + new string('0', 5 - i.ToString().Length) + i.ToString();
            //                    while (prod.FirstOrDefault(x => x.VendorCode.Equals(temp)) != null)
            //                    {
            //                        i++;
            //                        temp = K.VendorCode + new string('0', 5 - i.ToString().Length) + i.ToString();
            //                    }
            //                    PP.VendorCode = temp;
            //                    i++;
            //                }
            //            }
            //        }
            //    }
            //   db.SaveChanges();
            //}

            List<string> NotRel = new List<string>();
            List<string> Rel = new List<string>();
            List<string> Work = new List<string>();
            List<Konfig> KonList = new List<Konfig>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (string s in shops)
                {
                    try
                    {
                        var id = db.KonfigVe.FirstOrDefault(x => x.UserId.Equals(userId) & x.ShopName.Equals(s)).Id;
                        KonList.Add(db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(s)));
                        
                        Rel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & x.Relevance).ToList().Count.ToString());
                        NotRel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & !x.Relevance).ToList().Count.ToString());
                        Work.Add(db.KonfigVe.Where(x => x.ShopName.Equals(s)).First().Running.ToString());
                    }
                    catch
                    {
                        continue;
                    }
                }
                //db.Parameterses.RemoveRange(db.Parameterses.Where(x => String.IsNullOrEmpty(x.ProductPars.Id)));
                db.SaveChanges();

                
            }
            ViewBag.Rel = Rel;
            ViewBag.NotRel = NotRel;
            ViewBag.Work = Work;
            return View(KonList);
        }
        
        [HttpPost]
        public ActionResult Create(string name, string Number, string Phone)
        {
            var userId = User.Identity.GetUserId();
            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);

            Directory.CreateDirectory(user.FullName + @"\" + name);
            Directory.CreateDirectory(user.FullName + @"\" + name + @"\Photos");

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var a = new Konfig(name, userId) { VendorCode = Number, Phone = Phone };
                db.KonfigVe.Add(a);
                db.SaveChanges();
            }
            List<string> shops = new List<string>();
            foreach (DirectoryInfo p in user.GetDirectories().ToList())
            {
                shops.Add(p.Name);
            }
            List<string> NotRel = new List<string>();
            List<string> Rel = new List<string>();
            List<string> Work = new List<string>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (string s in shops)
                {
                    var id = db.KonfigVe.FirstOrDefault(x => x.UserId.Equals(userId) & x.ShopName.Equals(s)).Id;
                    Rel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & x.Relevance).ToList().Count.ToString());
                    NotRel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & !x.Relevance).ToList().Count.ToString());
                    Work.Add(db.KonfigVe.Where(x => x.ShopName.Equals(s)).First().Running.ToString());
                }
            }
            ViewBag.Rel = Rel;
            ViewBag.NotRel = NotRel;
            ViewBag.Work = Work;
            return View("Index", shops);
        }
        [HttpPost]
        public ActionResult CreateXML(string name, string Number, string Phone, IEnumerable<HttpPostedFileBase> files)
        {
            var userId = User.Identity.GetUserId();
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(@"C:\TempUsersFile\" + userId + file.FileName);
                }
                
            }
            string XMLPath = @"C:\TempUsersFile\" + userId + files.ToList()[0].FileName;
            string ExelPath = @"C:\TempUsersFile\" + userId + files.ToList()[1].FileName;
            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);
            List<ShopCategories> SC = new List<ShopCategories>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SC = db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList();
            }

            Directory.CreateDirectory(user.FullName + @"\" + name);
            Konfig tempKon = new Konfig(name, userId) { VendorCode = Number };

            FileInfo newFile = new FileInfo(ExelPath);
            ExcelPackage p = new ExcelPackage(newFile);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            List<ScrapingXML> tempScXML = new List<ScrapingXML>();
            int cell = 1;
            while (ws.Cells[1, cell].Text != "")
            {
                cell++;
            }
            int row = 2;
            while (ws.Cells[row, 1].Text != "")
            {
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                for (int i = 6; i < cell; i++)
                {
                    tempDic.Add(ws.Cells[1, i].Text, ws.Cells[row, i].Text);
                }
                tempScXML.Add(new ScrapingXML() { OldCat = ws.Cells[row, 1].Text, OurCat = ws.Cells[row, 2].Text, PromCat = ws.Cells[row, 3].Text, ExtraCharge = ws.Cells[row, 4].Text, Vendor = ws.Cells[row, 5].Text, par = tempDic });
                row++;
            }

            CQ cq = null;


            string contents = System.IO.File.ReadAllText(XMLPath);
            contents = contents.Replace("<param", "<p><param");
            contents = contents.Replace("</param>", "</param></p>");
            cq = CQ.CreateDocument(contents);
            List<ProductPars> pars = new List<ProductPars>();
            var bo = (cq.Find("yml_catalog>shop>offers>offer"));
            foreach (var MO in cq.Find("yml_catalog>shop>offers>offer"))
            {
                CQ temp = new CQ();
                temp = CQ.CreateDocument(MO.OuterHTML);

                string VendorCode = "";
                string OurCategori = "";
                string PortalCategoryId = "";
                string Price = "";
                string PriceOld = "";
                string Url = "";
                string Vendor = "";
                string Denomination = "";
                string Description = "";
                string KeyWords = "";
                bool rel = true;
                try
                {
                    VendorCode = temp.Find("vendorCode").Text();
                    OurCategori = temp.Find("categoryId").Text();
                    PortalCategoryId = temp.Find("portal_category_id").Text();
                    Price = temp.Find("price").Text();
                    PriceOld = temp.Find("price").Text();
                    Vendor = temp.Find("vendor").Text();
                    Denomination = temp.Find("name").Text();


                    if (Price.Contains("."))
                    {
                        Price = Price.Remove(Price.IndexOf("."));
                        PriceOld = Price;
                    }

                    try
                    {
                        if (String.IsNullOrEmpty(temp.Find("description").Text()))
                        {
                            Description = temp.Find("description").FirstElement().InnerHTML;
                            if (Description.Contains("<!--[CDATA["))
                            {
                                Description = Description.Replace("<!--[CDATA[", "");
                                Description = Description.Replace("]]-->", "");
                            }
                        }
                        else
                        {
                            foreach (var te in temp.Find("description"))
                            {
                                Description += te.OuterHTML;
                            }
                            Description = Description.Replace("<description>", "");
                            Description = Description.Replace("</description>", "");
                        }
                        Description = WebUtility.HtmlDecode(Description);
                    }
                    catch { }
                    KeyWords = temp.Find("keywords").Text();
                }
                catch
                {
                    rel = false;
                }
                List<Picture> pic = new List<Picture>();
                try
                {
                    foreach (var a in temp.Find("picture"))
                    {
                        if (WebUtility.HtmlDecode(a.InnerText).Contains(","))
                        {
                            foreach (var SplitP in WebUtility.HtmlDecode(a.InnerText).Split(new[] { ',' }))
                            {
                                pic.Add(new Picture(SplitP));
                            }
                        }
                        else
                        {
                            pic.Add(new Picture(WebUtility.HtmlDecode(a.InnerText)));
                        }

                    }
                }
                catch { if (pic.Count == 0) { rel = false; } }
                List<Parameters> par = new List<Parameters>();
                try
                {
                    foreach (var a in temp.Find("p"))
                    {
                        par.Add(new Parameters(a.FirstElementChild.Attributes["name"], WebUtility.HtmlDecode(a.InnerText), false));
                    }
                }
                catch { }

                ScrapingXML pair = tempScXML.FirstOrDefault(x => x.OldCat.Equals(OurCategori));
                if (pair != null)
                {
                    OurCategori = pair.OurCat;
                    PortalCategoryId = pair.PromCat;
                    if (!String.IsNullOrEmpty(pair.Vendor))
                    {
                        Vendor = pair.Vendor;
                    }
                    if (!pair.ExtraCharge.Equals("0"))
                    {
                        double K = Convert.ToDouble(pair.ExtraCharge);
                        double P = 0;
                        if (Price.Contains(".")) Price.Replace(".", ".");
                        try
                        {
                            P = Convert.ToDouble(Price);
                        }
                        catch
                        {

                        }
                        Price = Convert.ToString(Convert.ToInt32(P + ((P / 100) * K)));
                        PriceOld = Price;
                    }
                    foreach (var NewP in pair.par)
                    {
                        if (!String.IsNullOrEmpty(NewP.Value)) par.Add(new Parameters(NewP.Key, NewP.Value, false));
                    }

                    string KeyW = "";

                    List<string> KeyId = new List<string>() { OurCategori };
                    try
                    {
                        while (true)
                        {
                            if (!String.IsNullOrEmpty(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum))
                            {
                                KeyId.Add(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum);
                            }
                            else break;
                        }
                    }
                    catch { }

                    if (KeyId.Count != 0)
                    {
                        try
                        {
                            foreach (var c in KeyId)
                            {
                                KeyW += SC.FirstOrDefault(x => x.NumCat.Equals(c)).name + ", ";
                            }
                        }
                        catch { }
                    }
                    KeyWords += KeyW;
                }

                pars.Add(new ProductPars(userId, tempKon.Id, Number + VendorCode, OurCategori, PortalCategoryId, Vendor, Price, PriceOld, Url, Denomination, Description, KeyWords, pic, par) { Relevance = rel, Phone = Phone });
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.ProductsPars.AddRange(pars);
                db.KonfigVe.Add(tempKon);
                db.SaveChanges();
            }
            List<string> shops = new List<string>();
            foreach (DirectoryInfo s in user.GetDirectories().ToList())
            {
                shops.Add(s.Name);
            }

            return View("Index");
        }
        public ActionResult Delete(string shop)
        {
            var userId = User.Identity.GetUserId();
            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);

            Directory.Delete(user.FullName + @"\" + shop, true);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string id = db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(shop)).Id;
                db.ProductsPars.RemoveRange(db.ProductsPars.Where(x => x.ShopCode.Equals(id)).Include(x => x.Pictures).Include(x => x.Parameters));
                //Konfig CurKonfig = db.KonfigVe.Where(x => x.UserId.Equals(userId) & x.ShopName.Equals(shop))
                //    .Include(x => x.AdditionalPictures)
                //    .Include(x => x.Denominations)
                //    .Include(x => x.Descriptions)
                //    .Include(x => x.Excepts)
                //    .Include(x => x.Pagination)
                //    .Include(x => x.ProductUrl.Availability)
                //    .Include(x => x.ProductUrl.CheakPrise)
                //    .Include(x => x.PriceLow)
                //    .Include(x => x.PriceHigh)
                //    .Include(x => x.MainPicture)
                //    .Include(x => x.Availability)
                //    .Include(x => x.Vendor)
                //    .Include(x => x.Categores.Select(y => y.SubCatecories))
                //    .First();
                //CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                //db.KonfigVe.Remove(CurKonfig);
                db.SaveChanges();


               
            }




            List<string> shops = new List<string>();
            foreach (DirectoryInfo p in user.GetDirectories().ToList())
            {
                shops.Add(p.Name);
            }
            List<string> NotRel = new List<string>();
            List<string> Rel = new List<string>();
            List<string> Work = new List<string>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (string s in shops)
                {
                    var id = db.KonfigVe.FirstOrDefault(x => x.UserId.Equals(userId) & x.ShopName.Equals(s)).Id;
                    Rel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & x.Relevance).ToList().Count.ToString());
                    NotRel.Add(db.ProductsPars.Where(x => x.ShopCode.Equals(id) & !x.Relevance).ToList().Count.ToString());
                    Work.Add(db.KonfigVe.Where(x => x.ShopName.Equals(s)).First().Running.ToString());
                }
            }
            ViewBag.Rel = Rel;
            ViewBag.NotRel = NotRel;
            ViewBag.Work = Work;
            return View("Index", shops);
        }
        public async Task<bool> Run(string shop)
        {
            var userId = User.Identity.GetUserId();
            
            string id = "";
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Konfig K = db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(shop));
                if (K.Running) return false;
                else K.Running = true;
                db.Entry(K).State = EntityState.Modified;
                db.SaveChanges();
                id = db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(shop)).Id;
            }

            Process.Start(new DirectoryInfo(Server.MapPath("/")).FullName + @"Parser/DBPars.exe", id + " " + userId + " " + new DirectoryInfo(Server.MapPath("/")).FullName);

            return true;
        }

        public ActionResult AddToQueue(string id)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Konfig K = db.KonfigVe.FirstOrDefault(x => x.Id.Equals(id));
                K.InQueue = true;
                db.SaveChanges();
                return PartialView(K.InQueue);
            }
            
        }

        public async Task<bool> RunParseXML(string shop)
        {
            var a = new KonfigController();
            var userId = User.Identity.GetUserId();
            a.ProductPars(shop, userId);            
            return true;
        }

        public ActionResult AddToFileList(string Id)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Konfig K = db.KonfigVe.FirstOrDefault(x => x.Id.Equals(Id));
                K.ToFile = !(K.ToFile);
                db.SaveChanges();
                return PartialView(K);
            }

            
        }


        public void ToFile()
        {
            var userId = User.Identity.GetUserId();

            List<ProductPars> PP = new List<ProductPars>();
            List<ShopCategories> SC = new List<ShopCategories>();
            List<string> ShopIds = new List<string>();
            //System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", "");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ShopIds = db.KonfigVe.Where(x => x.ToFile).ToList().Select(x => x.Id).ToList();
                PP = db.ProductsPars.Where(x => x.UserId.Equals(userId) & x.Relevance & ShopIds.Contains(x.ShopCode)).Include(x=> x.Parameters).Include(x=> x.Pictures).ToList();
                SC = db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList();

            }
            int countAllProduct = 0;
            int FileCount = 1;
            int count = 0;
            bool FileFlag = true;
            
            while (FileFlag)
            {
                countAllProduct = 0;
                System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", "");
                string data = DateTime.Now.ToString().Replace("/", ".");
                string StartData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                    + "\r\n<!DOCTYPE yml_catalog SYSTEM \"shops.dtd\">"
                                    + "\r\n<yml_catalog date = \"" + data + "\">" +
                                    "\r\n<shop>\r\n<currencies>\r\n<currency id = \"UAH\" rate = \"1\" />\r\n</currencies>\r\n<categories>\r\n";
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", StartData, Encoding.UTF8);
                //TITLE

                foreach (ShopCategories Cat in SC)
                {
                    string C = "";
                    if (String.IsNullOrEmpty(Cat.parentNum)) C = "<category id=\"" + Cat.NumCat + "\">" + Cat.name + "</category>\r\n";
                    else C = "<category id=\"" + Cat.NumCat + "\" parentId=\"" + Cat.parentNum + "\">" + Cat.name + "</category>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", C, Encoding.UTF8);
                }
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", "</categories>\r\n<offers>\r\n", Encoding.UTF8);
                
                for (int i = count; i < PP.Count; i++)
                {
                    

                    if (PP[i].Description.Contains("\u001c\u001d"))
                    {
                        PP[i].Description = PP[i].Description.Replace("\u001c\u001d", "");
                    }
                    List<string> prod = new List<string>();
                    if (PP[i].Parameters.Where(x => x.Group == true).ToList().Count > 0)
                    {
                        int groupCount = 1;
                        foreach (Parameters par in PP[i].Parameters.Where(x => x.Group == true).ToList())
                        {
                            string strProd = "";
                            if (!String.IsNullOrEmpty(PP[i].GroupSKU))
                            {
                                strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode + "-" + groupCount.ToString(), PP[i].GroupSKU);
                            }
                            else
                            {
                                strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode + "-" + groupCount.ToString(), PP[i].VendorCode);
                            }
                            
                            if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                            if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                            strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                            if (PP[i].Description.Contains("<!--[CDATA["))
                            {
                                PP[i].Description = PP[i].Description.Replace("!--", "!");
                                strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                            }
                            else
                            {
                                strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                            }
                            if (PP[i].VendorCode.Contains("&"))
                            {
                                PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                            }
                            strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                            if (PP[i].Vendor.Contains("&"))
                            {
                                PP[i].Vendor = PP[i].Vendor.Replace("&", "&amp;");
                            }
                            strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                            strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                            strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                            if (PP[i].KeyWords.Contains("&"))
                            {
                                PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                            }
                            strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                            strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                            strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                            foreach (Picture pic in PP[i].Pictures)
                            {
                                strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                            }
                            foreach (Parameters paranm in PP[i].Parameters.Where(x => x.Group == false))
                            {
                                if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                                if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                                if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                                if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                                strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                            }
                            if (par.Name.Contains("\"")) par.Name = par.Name.Remove(par.Name.IndexOf("\""));
                            if (par.Value.Contains("\"")) par.Value = par.Value.Remove(par.Value.IndexOf("\""));
                            if (par.Name.Contains("&")) par.Name = par.Name.Replace("&", "&amp;");
                            if (par.Value.Contains("&")) par.Value = par.Value.Replace("&", "&amp;");
                            strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", par.Name, par.Value);
                            strProd += "</offer>\r\n";
                            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", strProd, Encoding.UTF8);
                            groupCount++;
                            countAllProduct++;
                        }
                    }
                    else
                    {
                        
                        string strProd = "";
                        if (!String.IsNullOrEmpty(PP[i].GroupSKU))
                        {
                            strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode, PP[i].GroupSKU);
                        }
                        else
                        {
                            strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\">\r\n", PP[i].VendorCode);
                        }
                        if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                        if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                        strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                        if (PP[i].Description.Contains("<!--[CDATA"))
                        {
                            PP[i].Description = PP[i].Description.Replace("!--", "!");
                            strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                        }
                        else
                        {
                            strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                        }
                        if (PP[i].VendorCode.Contains("&"))
                        {
                            PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                        try
                        {
                            if (PP[i].Vendor.Contains("&"))
                            {
                                PP[i].Vendor = PP[i].Vendor.Replace("&", "&amp;");
                            }
                        }
                        catch
                        {
                            var a = PP[i].Vendor;
                        }
                        strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                        strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                        strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                        if (PP[i].KeyWords.Contains("&"))
                        {
                            PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                        strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                        strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                        foreach (Picture pic in PP[i].Pictures)
                        {
                            strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                        }
                        foreach (Parameters paranm in PP[i].Parameters)
                        {
                            if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                            if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                            if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                            if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                            strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                        }
                        strProd += "</offer>\r\n";
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", strProd, Encoding.UTF8);
                        countAllProduct++;

                    }

                    if (countAllProduct > 10000)
                    {
                        string End = "</offers>\r\n</shop>\r\n</yml_catalog>\r\n";
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", End, Encoding.UTF8);
                        FileCount++;
                        count = i;
                        count++;
                        break;
                    }
                }
                if (countAllProduct < 10000)
                {
                    string End = "</offers>\r\n</shop>\r\n</yml_catalog>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom" + FileCount.ToString() + ".xml", End, Encoding.UTF8);
                    FileFlag = false;
                }
            }

            System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", "");
            string dataTime = DateTime.Now.ToString().Replace("/", ".");
            string StartDataFull = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                + "\r\n<!DOCTYPE yml_catalog SYSTEM \"shops.dtd\">"
                                + "\r\n<yml_catalog date = \"" + dataTime + "\">" +
                                "\r\n<shop>\r\n<currencies>\r\n<currency id = \"UAH\" rate = \"1\" />\r\n</currencies>\r\n<categories>\r\n";
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", StartDataFull, Encoding.UTF8);
            //TITLE

            foreach (ShopCategories Cat in SC)
            {
                string C = "";
                if (String.IsNullOrEmpty(Cat.parentNum)) C = "<category id=\"" + Cat.NumCat + "\">" + Cat.name + "</category>\r\n";
                else C = "<category id=\"" + Cat.NumCat + "\" parentId=\"" + Cat.parentNum + "\">" + Cat.name + "</category>\r\n";
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", C, Encoding.UTF8);
            }
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", "</categories>\r\n<offers>\r\n", Encoding.UTF8);

            for (int i = 0; i < PP.Count; i++)
            {
                if (PP[i].Description.Contains("\u001c\u001d"))
                {
                    PP[i].Description = PP[i].Description.Replace("\u001c\u001d", "");                    
                }
                List<string> prod = new List<string>();
                if (PP[i].Parameters.Where(x => x.Group == true).ToList().Count > 0)
                {
                    int groupCount = 1;
                    foreach (Parameters par in PP[i].Parameters.Where(x => x.Group == true).ToList())
                    {
                        string strProd = "";
                        strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode + "-" + groupCount.ToString(), PP[i].VendorCode);
                        if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                        if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                        strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                        if (PP[i].Description.Contains("<!--[CDATA["))
                        {
                            PP[i].Description = PP[i].Description.Replace("!--", "!");
                            strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                        }
                        else
                        {
                            strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                        }
                        if (PP[i].VendorCode.Contains("&"))
                        {
                            PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                        if (PP[i].Vendor.Contains("&"))
                        {
                            PP[i].Vendor = PP[i].Vendor.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                        strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                        strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                        if (PP[i].KeyWords.Contains("&"))
                        {
                            PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                        strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                        strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                        foreach (Picture pic in PP[i].Pictures)
                        {
                            strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                        }
                        foreach (Parameters paranm in PP[i].Parameters.Where(x => x.Group == false))
                        {
                            if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                            if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                            if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                            if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                            strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                        }
                        if (par.Name.Contains("\"")) par.Name = par.Name.Remove(par.Name.IndexOf("\""));
                        if (par.Value.Contains("\"")) par.Value = par.Value.Remove(par.Value.IndexOf("\""));
                        if (par.Name.Contains("&")) par.Name = par.Name.Replace("&", "&amp;");
                        if (par.Value.Contains("&")) par.Value = par.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", par.Name, par.Value);
                        strProd += "</offer>\r\n";
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", strProd, Encoding.UTF8);
                        groupCount++;

                    }
                }
                else
                {
                    string strProd = "";
                    strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\">\r\n", PP[i].VendorCode);
                    if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                    if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                    strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                    if (PP[i].Description.Contains("<!--[CDATA"))
                    {
                        PP[i].Description = PP[i].Description.Replace("!--", "!");
                        strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                    }
                    else
                    {
                        strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                    }
                    if (PP[i].VendorCode.Contains("&"))
                    {
                        PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                    strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                    strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                    if (PP[i].KeyWords.Contains("&"))
                    {
                        PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                    strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                    strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                    foreach (Picture pic in PP[i].Pictures)
                    {
                        strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                    }
                    foreach (Parameters paranm in PP[i].Parameters)
                    {
                        if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                        if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                        if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                        if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                    }
                    strProd += "</offer>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", strProd, Encoding.UTF8);
                }
            }
            string EndMain = "</offers>\r\n</shop>\r\n</yml_catalog>\r\n";
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", EndMain, Encoding.UTF8);

            // Общий файл 




            
            XmlDocument doc = new XmlDocument();
            doc.Load("https://sportbay.com.ua/yandex_market.xml?html_description=0&hash_tag=ffe71e7a3f2dfb5a01124f569bb80ee0&yandex_cpa=&group_ids=&exclude_fields=&label_ids=&sales_notes=&process_presence_sure=&product_ids=");


            Dictionary<string, string> UVC = new Dictionary<string, string>();
            foreach (XmlNode offer in doc.SelectNodes("yml_catalog/shop/offers/offer"))
            {
                string U = "";
                try
                {
                    U = offer.SelectSingleNode("./url").InnerText;

                }
                catch { }
                string V = "";
                try
                {
                    V = offer.SelectSingleNode("./vendorCode").InnerText;
                }
                catch { }
                try
                {
                    UVC.Add(V, U);
                }
                catch { continue; }
            }



            System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", "");

            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", StartDataFull, Encoding.UTF8);
            //TITLE

            foreach (ShopCategories Cat in SC)
            {
                string C = "";
                if (String.IsNullOrEmpty(Cat.parentNum)) C = "<category id=\"" + Cat.NumCat + "\">" + Cat.name + "</category>\r\n";
                else C = "<category id=\"" + Cat.NumCat + "\" parentId=\"" + Cat.parentNum + "\">" + Cat.name + "</category>\r\n";
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", C, Encoding.UTF8);
            }
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", "</categories>\r\n<offers>\r\n", Encoding.UTF8);

            for (int i = 0; i < PP.Count; i++)
            {

                string url = "";
                try
                {
                    if (String.IsNullOrEmpty(UVC.FirstOrDefault(x => x.Key.Equals(PP[i].VendorCode)).Value))
                    {
                        continue;
                    }
                    else
                    {
                        url = UVC.FirstOrDefault(x => x.Key.Equals(PP[i].VendorCode)).Value;
                    }
                }
                catch
                {
                    continue;
                }

                if (PP[i].Description.Contains("\u001c\u001d"))
                {
                    PP[i].Description = PP[i].Description.Replace("\u001c\u001d", "");
                }
                List<string> prod = new List<string>();
                if (PP[i].Parameters.Where(x => x.Group == true).ToList().Count > 0)
                {
                    int groupCount = 1;
                    foreach (Parameters par in PP[i].Parameters.Where(x => x.Group == true).ToList())
                    {
                        string strProd = "";
                        strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode + "-" + groupCount.ToString(), PP[i].VendorCode);
                        if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                        if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                        strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                        strProd += "<url>" + url + "</url>\r\n";
                        if (PP[i].Description.Contains("<!--[CDATA["))
                        {
                            PP[i].Description = PP[i].Description.Replace("!--", "!");
                            strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                        }
                        else
                        {
                            strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                        }
                        if (PP[i].VendorCode.Contains("&"))
                        {
                            PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                        if (PP[i].Vendor.Contains("&"))
                        {
                            PP[i].Vendor = PP[i].Vendor.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                        strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                        strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                        if (PP[i].KeyWords.Contains("&"))
                        {
                            PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                        strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                        strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                        foreach (Picture pic in PP[i].Pictures)
                        {
                            strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                        }
                        foreach (Parameters paranm in PP[i].Parameters.Where(x => x.Group == false))
                        {
                            if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                            if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                            if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                            if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                            strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                        }
                        if (par.Name.Contains("\"")) par.Name = par.Name.Remove(par.Name.IndexOf("\""));
                        if (par.Value.Contains("\"")) par.Value = par.Value.Remove(par.Value.IndexOf("\""));
                        if (par.Name.Contains("&")) par.Name = par.Name.Replace("&", "&amp;");
                        if (par.Value.Contains("&")) par.Value = par.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", par.Name, par.Value);
                        strProd += "</offer>\r\n";
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", strProd, Encoding.UTF8);
                        groupCount++;

                    }
                }
                else
                {
                    string strProd = "";
                    strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\">\r\n", PP[i].VendorCode);
                    if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                    if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                    strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                    strProd += "<url>" + url + "</url>\r\n";
                    if (PP[i].Description.Contains("<!--[CDATA"))
                    {
                        PP[i].Description = PP[i].Description.Replace("!--", "!");
                        strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                    }
                    else
                    {
                        strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                    }
                    if (PP[i].VendorCode.Contains("&"))
                    {
                        PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                    strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                    strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                    if (PP[i].KeyWords.Contains("&"))
                    {
                        PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                    strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                    strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                    foreach (Picture pic in PP[i].Pictures)
                    {
                        strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                    }
                    foreach (Parameters paranm in PP[i].Parameters)
                    {
                        if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                        if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                        if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                        if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                    }
                    strProd += "</offer>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", strProd, Encoding.UTF8);
                }
            }

            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_2.xml", EndMain, Encoding.UTF8);




            System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", "");

            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", StartDataFull, Encoding.UTF8);
            //TITLE

            foreach (ShopCategories Cat in SC)
            {
                string C = "";
                if (String.IsNullOrEmpty(Cat.parentNum)) C = "<category id=\"" + Cat.NumCat + "\">" + Cat.name + "</category>\r\n";
                else C = "<category id=\"" + Cat.NumCat + "\" parentId=\"" + Cat.parentNum + "\">" + Cat.name + "</category>\r\n";
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", C, Encoding.UTF8);
            }
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", "</categories>\r\n<offers>\r\n", Encoding.UTF8);

            for (int i = 0; i < PP.Count; i++)
            {

                string url = "";
                try
                {
                    if (String.IsNullOrEmpty(UVC.FirstOrDefault(x => x.Key.Equals(PP[i].VendorCode)).Value))
                    {
                        continue;
                    }
                    else
                    {
                        url = UVC.FirstOrDefault(x => x.Key.Equals(PP[i].VendorCode)).Value;
                    }
                }
                catch
                {
                    continue;
                }

                if (PP[i].Description.Contains("\u001c\u001d"))
                {
                    PP[i].Description = PP[i].Description.Replace("\u001c\u001d", "");
                }
                List<string> prod = new List<string>();
                
                    string strProd = "";
                    strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\">\r\n", PP[i].VendorCode);
                    if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                    if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                    strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                    strProd += "<url>" + url + "</url>\r\n";
                    if (PP[i].Description.Contains("<!--[CDATA"))
                    {
                        PP[i].Description = PP[i].Description.Replace("!--", "!");
                        strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                    }
                    else
                    {
                        strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                    }
                    if (PP[i].VendorCode.Contains("&"))
                    {
                        PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                    strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                    strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                    strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                    if (PP[i].KeyWords.Contains("&"))
                    {
                        PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                    strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                    strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                    foreach (Picture pic in PP[i].Pictures)
                    {
                        strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                    }
                    foreach (Parameters paranm in PP[i].Parameters)
                    {
                        if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                        if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                        if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                        if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                    }
                    strProd += "</offer>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", strProd, Encoding.UTF8);
                
            }

            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_ipopo.xml", EndMain, Encoding.UTF8);

        }

        public void ToFileFEX()
        {
            var userId = User.Identity.GetUserId();

            List<ProductPars> PP = new List<ProductPars>();
            List<ShopCategories> SC = new List<ShopCategories>();
            List<string> ShopIds = new List<string>();
            //System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_prom.xml", "");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ShopIds = db.KonfigVe.Where(x => x.ToFile).ToList().Select(x => x.Id).ToList();
                PP = db.ProductsPars.Where(x => x.UserId.Equals(userId) & x.Relevance & ShopIds.Contains(x.ShopCode)).Include(x => x.Parameters).Include(x => x.Pictures).ToList();
                SC = db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList();

            }
            int countAllProduct = 0;
            int FileCount = 1;
            int count = 0;
            bool FileFlag = true;

            

            System.IO.File.WriteAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", "");
            string dataTime = DateTime.Now.ToString().Replace("/", ".");
            string StartDataFull = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                + "\r\n<!DOCTYPE yml_catalog SYSTEM \"shops.dtd\">"
                                + "\r\n<yml_catalog date = \"" + dataTime + "\">" +
                                "\r\n<shop>\r\n<currencies>\r\n<currency id = \"UAH\" rate = \"1\" />\r\n</currencies>\r\n<categories>\r\n";
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", StartDataFull, Encoding.UTF8);
            //TITLE

            foreach (ShopCategories Cat in SC)
            {
                string C = "";
                if (String.IsNullOrEmpty(Cat.parentNum)) C = "<category id=\"" + Cat.NumCat + "\">" + Cat.name + "</category>\r\n";
                else C = "<category id=\"" + Cat.NumCat + "\" parentId=\"" + Cat.parentNum + "\">" + Cat.name + "</category>\r\n";
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", C, Encoding.UTF8);
            }
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", "</categories>\r\n<offers>\r\n", Encoding.UTF8);

            for (int i = 0; i < PP.Count; i++)
            {
                if (PP[i].Description.Contains("\u001c\u001d"))
                {
                    PP[i].Description = PP[i].Description.Replace("\u001c\u001d", "");
                }
                List<string> prod = new List<string>();
                if (PP[i].Parameters.Where(x => x.Group == true).ToList().Count > 0)
                {
                    int groupCount = 1;
                    foreach (Parameters par in PP[i].Parameters.Where(x => x.Group == true).ToList())
                    {
                        string strProd = "";
                        strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\" group_id=\"{1}\">\r\n", PP[i].VendorCode + "-" + groupCount.ToString(), PP[i].VendorCode);
                        if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                        if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                        strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                        if (PP[i].Description.Contains("<!--[CDATA["))
                        {
                            PP[i].Description = PP[i].Description.Replace("!--", "!");
                            strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                        }
                        else
                        {
                            strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                        }
                        if (PP[i].VendorCode.Contains("&"))
                        {
                            PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                        if (PP[i].Vendor.Contains("&"))
                        {
                            PP[i].Vendor = PP[i].Vendor.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<vendor>{0}</vendor>\r\n", PP[i].Vendor);
                        strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                        strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                        if (PP[i].KeyWords.Contains("&"))
                        {
                            PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                        }
                        strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                        strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                        strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                        foreach (Picture pic in PP[i].Pictures)
                        {
                            strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                        }
                        foreach (Parameters paranm in PP[i].Parameters.Where(x => x.Group == false))
                        {
                            if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                            if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                            if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                            if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                            strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                        }
                        if (par.Name.Contains("\"")) par.Name = par.Name.Remove(par.Name.IndexOf("\""));
                        if (par.Value.Contains("\"")) par.Value = par.Value.Remove(par.Value.IndexOf("\""));
                        if (par.Name.Contains("&")) par.Name = par.Name.Replace("&", "&amp;");
                        if (par.Value.Contains("&")) par.Value = par.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", par.Name, par.Value);
                        strProd += "</offer>\r\n";
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", strProd, Encoding.UTF8);
                        groupCount++;

                    }
                }
                else
                {
                    string strProd = "";
                    strProd += String.Format("<offer id = \"{0}\" available=\"true\" selling_type=\"r\">\r\n", PP[i].VendorCode);
                    if (PP[i].Denomination.Contains("<")) PP[i].Denomination = PP[i].Denomination.Remove(PP[i].Denomination.IndexOf("<"));
                    if (PP[i].Denomination.Contains("&")) PP[i].Denomination = PP[i].Denomination.Replace("&", "&amp;");
                    strProd += String.Format("<name><![CDATA[{0}]]></name>\r\n", PP[i].Denomination);
                    if (PP[i].Description.Contains("<!--[CDATA"))
                    {
                        PP[i].Description = PP[i].Description.Replace("!--", "!");
                        strProd += String.Format("<description>{0}</description>\r\n", PP[i].Description);
                    }
                    else
                    {
                        strProd += String.Format("<description><![CDATA[{0}]]></description>\r\n", PP[i].Description);
                    }
                    if (PP[i].VendorCode.Contains("&"))
                    {
                        PP[i].VendorCode = PP[i].VendorCode.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<vendorCode>{0}</vendorCode>\r\n", PP[i].VendorCode);
                    strProd += String.Format("<categoryId>{0}</categoryId>\r\n", PP[i].OurCategori);
                    strProd += String.Format("<portal_category_id>{0}</portal_category_id>\r\n", PP[i].PortalCategoryId);
                    if (PP[i].KeyWords.Contains("&"))
                    {
                        PP[i].KeyWords = PP[i].KeyWords.Replace("&", "&amp;");
                    }
                    strProd += String.Format("<keywords>{0}</keywords>\r\n", PP[i].KeyWords);
                    strProd += String.Format("<price>{0}</price>\r\n", PP[i].Price);
                    strProd += String.Format("<oldprice>{0}</oldprice>\r\n", PP[i].PriceOld);
                    foreach (Picture pic in PP[i].Pictures)
                    {
                        strProd += String.Format("<picture>{0}</picture>\r\n", pic.url);
                    }
                    foreach (Parameters paranm in PP[i].Parameters)
                    {
                        if (paranm.Name.Contains("\"")) paranm.Name = paranm.Name.Remove(paranm.Name.IndexOf("\""));
                        if (paranm.Value.Contains("\"")) paranm.Value = paranm.Value.Remove(paranm.Value.IndexOf("\""));
                        if (paranm.Name.Contains("&")) paranm.Name = paranm.Name.Replace("&", "&amp;");
                        if (paranm.Value.Contains("&")) paranm.Value = paranm.Value.Replace("&", "&amp;");
                        strProd += String.Format("<param name=\"{0}\" unit= \"\">{1}</param>\r\n", paranm.Name, paranm.Value);
                    }
                    strProd += "</offer>\r\n";
                    System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", strProd, Encoding.UTF8);
                }
            }
            string EndMain = "</offers>\r\n</shop>\r\n</yml_catalog>\r\n";
            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products_FEX.xml", EndMain, Encoding.UTF8);

            // Общий файл 
            
        }

        public void ToFileCSV()
        {
            var userId = User.Identity.GetUserId();

            List<ProductPars> PP = new List<ProductPars>();
            List<ShopCategories> SC = new List<ShopCategories>();
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                PP = db.ProductsPars.AsNoTracking().Where(x => x.UserId.Equals(userId) & x.Relevance).Include(x => x.Pictures).Include(x => x.Parameters).ToList();
                

            }
            //Title	Short-content Full - content	Category	Brand	SKU	Parent SKU	Count	Price	Sale-price

            var ParName = PP.SelectMany(x => x.Parameters).GroupBy(x => x.Name).Select(x => x.Key).ToList();
            int countAllProduct = 0;
            int FileCount = 1;
            int count = 0;
            bool FileFlag = true;
            var sdfsd = PP.Where(x => !x.Price.Equals("")).ToList();

            var nChunks = 4;
            var totalLength = PP.Count();
            var chunkLength = (int)Math.Ceiling(totalLength / (double)nChunks);

            var parts = Enumerable.Range(0, nChunks).Select(i => PP.Skip(i * chunkLength).Take(chunkLength).ToList()).ToList();


            
            foreach (var PL in parts)
            {
                System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products" + parts.IndexOf(PL).ToString() + ".csv", "Title;Short-content;Full-content;Category;Brand;SKU;Parent SKU;Count;Price;Sale-price;" + String.Join(";", ParName.Where(x => !x.Equals("")).Select(x => "H-" + x).ToArray()) + ";img\n", Encoding.UTF8);

                foreach (var p in PL)
                {
                    p.Denomination = p.Denomination.Replace(p.Vendor + " ", "");
                    p.Denomination += " " + p.Vendor;
                    p.Description = p.Description.Replace("\n", "");
                    p.Description = p.Description.Replace("\r\n", "");
                    p.Description = p.Description.Replace(Environment.NewLine, "");
                    p.Description = p.Description.Replace(";", "");
                    var S = p.Parameters.Where(x => x.Name.Equals("Размеры")).ToList();
                    if (S.Count > 1)
                    {
                        string MainData = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};", p.Denomination, "", p.Description, p.KeyWords.Replace(",", ""), p.Vendor, p.VendorCode, "", "", p.PriceOld, p.Price);//первый с разновидностей
                        string SizeVal = String.Join("|", S.Select(x => x.Value.Replace(" UKR","")).ToArray());
                        List<string> ParValList = new List<string>();
                        foreach (var Name in ParName.Where(x => !x.Equals("")).ToList())
                        {
                            if (Name.Equals("Размеры"))
                            {
                                ParValList.Add(SizeVal);
                            }
                            else
                            {
                                if (p.Parameters.FirstOrDefault(x => x.Name.Equals(Name)) != null)
                                {
                                    ParValList.Add(p.Parameters.FirstOrDefault(x => x.Name.Equals(Name)).Value.Replace(";", " ").Replace(",","|"));
                                }
                                else
                                {
                                    ParValList.Add("");
                                }
                            }
                        }
                        string Img = String.Join(", ", p.Pictures.Select(x => x.url).ToArray());

                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products" + parts.IndexOf(PL).ToString()+ ".csv",
                                                                        MainData + String.Join(";", ParValList.ToArray()) + ";" + Img + "\n",
                                                                        Encoding.UTF8);
                        ParValList.Clear();
                        foreach (var Size in S)
                        {
                            string SpeciesData = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};", p.Denomination, "", p.Description, p.KeyWords.Replace(",", ""), p.Vendor, "", p.VendorCode, "", p.PriceOld, p.Price);//разновидностей
                            var a = p.Parameters.Where(y => y.Name.Equals("Размеры") & !y.Value.Equals(Size.Value)).ToList();
                            var TPar = p.Parameters.Where(x => !a.Contains(x)).ToList();
                            foreach (var Name in ParName.Where(x => !x.Equals("")).ToList())
                            {
                                if (TPar.FirstOrDefault(x => x.Name.Equals(Name)) != null)
                                {
                                    ParValList.Add(TPar.FirstOrDefault(x => x.Name.Equals(Name)).Value.Replace(";", " ").Replace(" UKR", "").Replace(",", "|"));
                                }
                                else
                                {
                                    ParValList.Add("");
                                }
                            }
                            System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products" + parts.IndexOf(PL).ToString()+ ".csv",
                                                                        SpeciesData + String.Join(";", ParValList.ToArray()) + ";" + "\n",
                                                                        Encoding.UTF8);
                            ParValList.Clear();
                        }
                    }
                    else
                    {
                        string MainData = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};", p.Denomination, "", p.Description, p.KeyWords.Replace(",", ""), p.Vendor, p.VendorCode, "", "", p.PriceOld, p.Price);
                        List<string> ParValList = new List<string>();
                        foreach (var Name in ParName.Where(x => !x.Equals("")).ToList())
                        {
                            if (p.Parameters.FirstOrDefault(x => x.Name.Equals(Name)) != null)
                            {
                                ParValList.Add(p.Parameters.FirstOrDefault(x => x.Name.Equals(Name)).Value.Replace(";", " ").Replace(" UKR", "").Replace(",", "|"));
                            }
                            else
                            {
                                ParValList.Add("");
                            }
                        }
                        string Img = String.Join(", ", p.Pictures.Select(x => x.url).ToArray());
                        System.IO.File.AppendAllText(new DirectoryInfo(Server.MapPath("/")).FullName + @"\Users\" + userId + @"\products" + parts.IndexOf(PL).ToString()+ ".csv",
                                                                        MainData + String.Join(";", ParValList.ToArray()) + ";" + Img + "\n",
                                                                        Encoding.UTF8);
                    }

                }
            }


        }
    }
}

//<offers>
//<offer id = "0102788-1" available="true" selling_type="r" group_id="0102788">
//<vendorCode>0102788</vendorCode>
//<name>Лыжи горные LINE SOULMATE 98 2015</name>
//<description>Женские лыжи Soulmate представляют такую же эффективность и высочайшие характеристики, как и у мужских фрирайдовых лыж.Универсальная модель "всё в одном", точная, маневренная и жесткая.</description>
//<picture>http://sshop.com.ua/Photos/0102788.jpg</picture>
//<picture>http://sshop.com.ua/Photos/0102788_1.jpg</picture>
//<picture>http://sshop.com.ua/Photos/0102788_2.jpg</picture>
//<categoryId>64</categoryId>
//<price>3359</price>
//<oldprice>3359</oldprice>
//<portal_category_id>202401</portal_category_id>
//<param name = "Назначение" unit="">Горные лыжи</param>
//  <param name = "Уровень катания" unit= "" > Экспертный </ param >
//  < param name= "Назначение" unit= "" > Фристайл(Freeski) </ param >
//  < param name= "Размер" unit= "" > 165 </ param >
//  < keywords > Лыжи, Лыжи, Спорттовары Горные лыжи</keywords>
//  </offer>