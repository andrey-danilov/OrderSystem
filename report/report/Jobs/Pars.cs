using Quartz;
using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace report.Jobs
{
    public class Pars : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task Execute(IJobExecutionContext context)
        {
            
            List<Konfig> CurKonfigs = new List<Konfig>();
            WebClient webClient = new WebClient();
            CurKonfigs = db.KonfigVe.Where(x => x.InQueue).ToList();


            foreach (var CK in CurKonfigs)
            {
                CK.InQueue = false;
                CK.Running = true;
                db.SaveChanges();
            }
                

            foreach (var CK in CurKonfigs)
            {

                List<ProductPars> PP = new List<ProductPars>();
                List<AutoCorrect> AU = null;
                List<ShopCategories> ShC = null;
                Konfig CurKonfig = new Konfig();
                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(CK.Id))
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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                var userId = CurKonfig.UserId;

                if (CurKonfig.Contacts.Count == 0)
                {
                    CurKonfig.Contacts.Add(new Contacts() { Konfig = CurKonfig, Id = Guid.NewGuid().ToString() });
                }

                foreach (var SuCat in CurKonfig.Categores)
                {
                    foreach (var SC in SuCat.SubCatecories)
                    {
                        db.Entry(SC).Collection(x => x.KeyWord).Load();
                        foreach (var Ch in SC.Characteristics)
                        {

                            foreach (var Ex in Ch.Exceptions) { }
                        }
                    }
                }

                Console.WriteLine(CurKonfig.Categores.Count);


                PP = db.ProductsPars.Where(x => x.ShopCode.Contains(CurKonfig.Id)).Include(x => x.Pictures).Include(x => x.Parameters).ToList();

                string PicPath = @"C:\inetpub\wwwroot" + @"\Users\" + userId + @"\" + CurKonfig.ShopName + @"\Photos\";


                List<Models.Picture> Pic = PP.SelectMany(x => x.Pictures).ToList();
                Pic.ForEach(s => s.ProductsPars = null);
                List<Models.Parameters> Par = PP.SelectMany(x => x.Parameters).ToList();
                
                db.Parameterses.RemoveRange(Par);
                
                int count = 0;
                try
                {
                    count = PP.Select(x => Int32.Parse(x.VendorCode.Remove(0, 2))).OrderByDescending(x => x).First();

                }
                catch
                {
                    count = 1;
                }
                var tempPic = new List<Models.Picture>();


                foreach (var p in PP)
                {
                    p.Relevance = false;

                }

                

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(CurKonfig.Pagination.value);
                }
                catch
                {
                }
                
                foreach (XmlNode node in doc.DocumentElement.SelectNodes(CurKonfig.ProductUrl.value))
                {
                    var product = PP.FirstOrDefault(x => x.Denomination.Equals(node.SelectSingleNode(CurKonfig.Denominations[0].value).InnerText)) ;
                    if (product == null) product = new Models.ProductPars();
                    try
                    {
                        product.Denomination = node.SelectSingleNode(CurKonfig.Denominations[0].value).InnerText;
                        product.Description = node.SelectSingleNode(CurKonfig.Descriptions[0].value).InnerText;
                        product.PriceOld = node.SelectSingleNode(CurKonfig.PriceLow.value).InnerText;
                        product.Pictures = new List<Models.Picture>();
                        product.Parameters = new List<Models.Parameters>();
                        product.OurCategori = node.SelectSingleNode("./categoryId").InnerText;
                        try
                        {
                            product.Vendor = node.SelectSingleNode(CurKonfig.Vendor.value).InnerText;

                        }
                        catch
                        {
                            product.Vendor = "";
                        }
                        product.Url = CurKonfig.Availability.value + product.Denomination;
                        product.ShopCode = CurKonfig.Id;
                        product.UserId = CurKonfig.UserId;
                    }
                    catch
                    {
                        var dfd = "dfdf";
                    }
                    if (product.Id == null)
                    {
                        product.Id = Guid.NewGuid().ToString();
                        product.VendorCode = CurKonfig.VendorCode + new string('0', 5 - count.ToString().Length) + count.ToString();
                        count++;
                        PP.Add(product);
                    }
                    var urlMP = node.SelectNodes(CurKonfig.MainPicture.value);
                    int i = 1;
                    foreach(XmlNode pic in urlMP)
                    {
                        string format = pic.InnerText.Substring(pic.InnerText.LastIndexOf("."));
                        try
                        {
                            webClient.DownloadFile(pic.InnerText, PicPath + product.VendorCode + "_" + i.ToString() + format);
                            product.Pictures.Add(new Models.Picture("http://sshop.com.ua/Users/" + userId + "/" + CurKonfig.ShopName + "/Photos/" + product.VendorCode + "_"+ i.ToString()+ format));
                            i++;
                        }
                        catch
                        {

                        }
                    }
                    db.Pictures.AddRange(product.Pictures);
                }
               
                foreach (var cat in CurKonfig.Categores)
                {
                    var Prod = PP.Where(x => x.OurCategori.Equals(cat.url)).ToList();
                    var asdf = 0;
                    foreach (var s in cat.SubCatecories)
                    {
                        try
                        {
                            double k = 0;

                            try
                            {
                                k = Convert.ToDouble(s.price_processing);
                            }
                            catch
                            {
                                k = 0;
                            }


                            foreach (var p in Prod)
                            {
                                p.Relevance = true;

                                List<string> KWS = new List<string>();
                                try
                                {
                                    foreach (KeyWord KW in s.KeyWord)
                                    {
                                        try
                                        {
                                            string a = (KW.CheakVendor && KW.Name.ToLower().Contains(p.Vendor.ToLower())) ? KW.Name : KW.Name;
                                            a = (KW.StringFormat) ? String.Format(KW.Name, p.Vendor) : a;
                                            KWS.Add(a);
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                    }
                                }
                                catch
                                {

                                }
                                p.KeyWords = String.Join(", ", KWS.ToArray());
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
                    foreach(var cp in Prod)
                    {
                        db.ProductsPars.AddOrUpdate(cp);
                    }
                }
                
                CurKonfig.Running = false;
                db.SaveChanges();
            }


        }
    }
}