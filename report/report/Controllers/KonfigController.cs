using Hangfire;
using Microsoft.AspNet.Identity;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using PagedList;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System.Data.Entity;
using System.Net;

namespace report.Controllers
{
    public class KonfigController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ViewResult Index(string shop, int? page)
        {
            var userId = User.Identity.GetUserId();
            Konfig CurKonfig = new Konfig();
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurKonfig = db.KonfigVe.Where(x => x.ShopName.Equals(shop) & x.UserId.Equals(userId))
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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                
            }
            //DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);

            //Directory.CreateDirectory(user.FullName + @"\" + shop);

            //XmlDocument doc = new XmlDocument();

            //List<ProductPars> PP = new List<ProductPars>();

            //doc.Load(user.FullName + @"\" + shop + @"\products_list.xml");


            //foreach (XmlNode PL in doc.SelectNodes("/PL/product"))
            //{
            //    string ur = PL.Attributes["url"].Value;
            //    if (ur.Contains("&amp;")) ur = ur.Replace("&amp;", "&");
            //    PP.Add(new ProductPars() { Parameters = new List<report.Models.Parameters>(), Pictures = new List<report.Models.Picture>(), Id = Guid.NewGuid().ToString(), VendorCode = PL.Attributes["Id"].Value, Url = ur, ShopCode = CurKonfig.Id, UserId = userId, Relevance = false });
            //}

            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    db.ProductsPars.RemoveRange(db.ProductsPars.Where(x => x.ShopCode.Equals(CurKonfig.Id)).ToList());
            //    db.ProductsPars.AddRange(PP);
            //    db.SaveChanges();
            //}

            //DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + "/Users/" + userId);
            //XmlDocument doc = new XmlDocument();
            //doc.Load(user.FullName + @"\yandex_market.xml");
            //Dictionary<string, string> NameSKU = new Dictionary<string, string>();
            //foreach (XmlNode MO in doc.SelectNodes("offers/offer"))
            //{
            //    string name = MO.SelectSingleNode(".//name").InnerText;
            //    if (name.IndexOf("(Код:") > 0) name = name.Remove(name.IndexOf("(Код:"));
            //    string Sku = MO.SelectSingleNode(".//vendorCode").InnerText;

            //    try
            //    {
            //        if (CurKonfig.VendorCode.Equals(Sku.Remove(2)))
            //        {
            //            NameSKU.Add(name, Sku);
            //        }
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}
            //KeyValuePair<string, string> aa = NameSKU.FirstOrDefault(x => x.Value.Equals("061257"));
            
            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    List<ProductPars> prod = db.ProductsPars.Where(x => x.ShopCode.Equals(CurKonfig.Id)).Include(x => x.Parameters).Include(x => x.Pictures).ToList();

            //    List<string> prodUpID = new List<string>();
            //    if (prod.Count > 0)
            //    {
            //        //foreach (ProductPars PP in prod)
            //        //{
            //        //    try
            //        //    {
            //        //        KeyValuePair<string, string> a = NameSKU.FirstOrDefault(x => x.Key.ToUpper().Equals(PP.Denomination.ToUpper()));
            //        //        if (a.Key != null)
            //        //        {
            //        //            PP.VendorCode = a.Value;
            //        //            NameSKU.Remove(a.Key);
            //        //            prodUpID.Add(PP.Id);
            //        //        }
            //        //        else
            //        //        {
            //        //            PP.VendorCode = "0";
            //        //        }
            //        //    }
            //        //    catch
            //        //    {

            //        //    }
            //        //}
            //        //db.SaveChanges();
            //        int i = 1;

            //        var a = prod.Where(x => x.VendorCode.Equals("0")).ToList();
            //        foreach (ProductPars PP in prod.Where(x => x.VendorCode.Equals("0")).ToList())
            //        {
            //            //if (prodUpID.FirstOrDefault(x => x.Equals(PP.Id)) != null)
            //            //{
            //                string temp = CurKonfig.VendorCode + new string('0', 5 - i.ToString().Length) + i.ToString();
            //                while (prod.FirstOrDefault(x => x.VendorCode.Equals(temp)) != null)
            //                {
            //                    i++;
            //                    temp = CurKonfig.VendorCode + new string('0', 5 - i.ToString().Length) + i.ToString();
            //                }
            //                PP.VendorCode = temp;

            //                i++;
                        
            //            //}
            //        }
            //        db.SaveChanges();

            //    }

            //}

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            
            return View(CurKonfig);
        }

        [HttpPost]
        public ActionResult EditPhone(Contacts ob)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Konfig CurKonfig = new Konfig();
                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(ob.Konfig.Id))
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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                ob.Konfig = CurKonfig;
                if (db.Contacts.FirstOrDefault(x => x.Id.Equals(ob.Id)) != null)
                {
                    Contacts Con = db.Contacts.FirstOrDefault(x => x.Id.Equals(ob.Id));
                    Con.name = ob.name;
                    Con.value = ob.value;
                    Con.description = ob.description;
                    db.Entry(Con).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return PartialView("EditPhone", ob);
            }
        }

        [HttpPost]
        public ActionResult AddPhone(Contacts ob)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Konfig CurKonfig = new Konfig();
                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(ob.Konfig.Id))
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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                ob.Konfig = CurKonfig;
                ob.Id = Guid.NewGuid().ToString();
                db.Entry(ob).State = EntityState.Added;
                db.SaveChanges();
                return PartialView("EditPhone", ob);
            }
        }

        [HttpPost]
        public ViewResult EditProductUrl(ProductUrl ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ProductUrl res = db.ProductUrls.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                res.take = ob.take;
                res.by = ob.by;
                res.value = ob.value;

                db.Entry(res).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.ProductUrl.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }

            return View("Index", CurKonfig);
        }

        [HttpPost]
        public ViewResult EditPagination(Pagination ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Pagination res = db.Paginations.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                //res.take = ob.take;
                //res.by = ob.by;
                //res.value = ob.value;

                //db.Entry(res).State = EntityState.Modified;
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.Pagination.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();



            }

            return View("Index", CurKonfig);
        }


        [HttpPost]
        public ViewResult EditAvailability(Availability ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Availability res = db.Availabilities.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                //res.take = ob.take;
                //res.by = ob.by;
                //res.value = ob.value;

                //db.Entry(res).State = EntityState.Modified;
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.Availability.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }

            return View("Index", CurKonfig);
        }

        [HttpPost]
        public ViewResult EditPriceLow(PriceLow ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //PriceLow res = db.PriceLows.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                //res.take = ob.take;
                //res.by = ob.by;
                //res.value = ob.value;

                //db.Entry(res).State = EntityState.Modified;
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.PriceLow.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditPriceHigh(PriceHigh ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //PriceHigh res = db.PriceHighs.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                //res.take = ob.take;
                //res.by = ob.by;
                //res.value = ob.value;


                //db.Entry(res).State = EntityState.Modified;
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.PriceHigh.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditMainPicture(MainPicture ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //MainPicture res = db.MainPictures.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                //res.take = ob.take;
                //res.by = ob.by;
                //res.value = ob.value;
                //res.format = ob.format;
                //res.empty = ob.empty;

                //db.Entry(res).State = EntityState.Modified;
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.MainPicture.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditVendor(Vendor ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.VendorSearch.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();


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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }
        public ViewResult EditVendorSearch(Vendor ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Vendor res = db.Vendors.Where(x => x.Id.Equals(ob.Id)).FirstOrDefault();
                res.take = ob.take;
                res.by = ob.by;
                res.value = ob.value;

                db.Entry(res).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.Vendor.Id.Equals(ob.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditDenominations(Denomination ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Denomination res = db.Denominations.Where(x => x.Id.Equals(ob.Id)).Include(x => x.Konfig).FirstOrDefault();
                res.take = ob.take;
                res.by = ob.by;
                res.value = ob.value;

                db.Entry(res).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(ob.Konfig.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();


            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditDescriptions(Description ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Description res = db.Descriptions.Where(x => x.Id.Equals(ob.Id)).Include(x => x.Konfig).FirstOrDefault();
                res.take = ob.take;
                res.by = ob.by;
                res.value = ob.value;

                db.Entry(res).State = EntityState.Modified;
                db.SaveChanges();

                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(ob.Konfig.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();



            }
            return View("Index", CurKonfig);
        }
        public ViewResult AddDescriptions(string id)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                CurKonfig.Descriptions.Add(new Description(false));
                db.SaveChanges();

            }
            return View("Index", CurKonfig);
        }

        public ViewResult EditAdditionalPictures(AdditionalPicture ob)
        {
            Konfig CurKonfig = new Konfig();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                //AdditionalPicture res = db.AdditionalPictures.Where(x => x.Id.Equals(ob.Id)).Include(x => x.Konfig).FirstOrDefault();

                //res.format = ob.format;
                //res.by = ob.by;
                //res.empty = ob.empty;
                //res.take = ob.take;
                //res.value = ob.value;

                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();
                CurKonfig = db.KonfigVe.Where(x => x.Id.Equals(ob.Konfig.Id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

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
                CurKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();




            }
            ViewBag.MainCharacteristics = CurKonfig.MainCharacteristics;
            return View("Index", CurKonfig);
        }




        //public ActionResult Index2(string shop)
        //{
        //    var userId = User.Identity.GetUserId();
        //    Konfig CurrentKonfig = new Konfig();
        //    List<Category> CurrentCategory = new List<Category>();
        //    if (Session["kon"] as Konfig != null)
        //    {
        //        CurrentKonfig = Session["kon"] as Konfig;
        //        CurrentCategory = Session["konLow"] as List<Category>;
        //    }
        //    else
        //    {

        //        DirectoryInfo path = new DirectoryInfo(Server.MapPath("/") + "/Users/" + userId + "/" + shop);
        //        var document = new StreamReader(path.FullName + @"\Konfig.xml", Encoding.UTF8, false);
        //        var contents = document.ReadToEnd();
        //        string EncodedString = contents.Replace("&", "&amp;");
        //        doc.LoadXml(EncodedString);
        //        FilePath = path.FullName + @"\Konfig.xml";
        //        CurrentKonfig.MainOptions = new List<MainOption>();
        //        foreach (XmlNode MO in doc.SelectNodes("/shop/main_options"))
        //        {
        //            CurrentKonfig.MainOptions.Add(new MainOption(
        //                                                MO.Attributes["name"].Value,
        //                                                MO.Attributes["By"].Value,
        //                                                MO.Attributes["take"].Value,
        //                                                MO.InnerText));
        //        }



        //        CurrentKonfig.MainCharacteristics = new List<Characteristic>();
        //        int q = 0;
        //        foreach (XmlNode MC in doc.SelectNodes("/shop/characteristics"))
        //        {
        //            List<Except> ex = new List<Except>();

        //            foreach (XmlNode Exep in MC.SelectNodes(".//exception"))
        //            {
        //                ex.Add(new Except(Exep.Attributes["name"].InnerText,
        //                                    Exep.InnerText));
        //            }

        //            CurrentKonfig.MainCharacteristics.Add(new Characteristic(q.ToString(),
        //                                                                    Convert.ToBoolean(MC.Attributes["group"].InnerText),
        //                                                                    MC.SelectSingleNode(".//main").InnerText,
        //                                                                    MC.SelectSingleNode(".//main").Attributes["By"].InnerText,
        //                                                                    MC.SelectSingleNode(".//name").InnerText,
        //                                                                    MC.SelectSingleNode(".//name").Attributes["By"].InnerText,
        //                                                                    MC.SelectSingleNode(".//value").InnerText,
        //                                                                    MC.SelectSingleNode(".//value").Attributes["By"].InnerText,
        //                                                                    ex
        //                                                                    ));
        //            q++;
        //        }


        //        CurrentKonfig.Categores = new List<Category>();
        //        int i = 0;

        //        foreach (XmlNode Cat in doc.SelectNodes("/shop/categories/category"))
        //        {
        //            List<SubCatecory> TempSubCatecory = new List<SubCatecory>();
        //            foreach (XmlNode SC in Cat.SelectNodes(".//SubCatecory"))
        //            {
        //                bool mainFlaf = false;
        //                List<Characteristic> ListCharectCat = new List<Characteristic>();
        //                int j = 0;
        //                foreach (XmlNode CH in SC.SelectNodes(".//characteristics"))
        //                {
        //                    List<Except> ex = new List<Except>();
        //                    foreach (XmlNode Exep in CH.SelectNodes(".//exception"))
        //                    {
        //                        ex.Add(new Except(Exep.Attributes["name"].InnerText,
        //                                            Exep.InnerText));
        //                    }
        //                    Characteristic temp = new Characteristic(j.ToString(),
        //                                                        Convert.ToBoolean(CH.Attributes["group"].InnerText),
        //                                                        CH.SelectSingleNode(".//main").InnerText,
        //                                                        CH.SelectSingleNode(".//main").Attributes["By"].InnerText,
        //                                                        CH.SelectSingleNode(".//name").InnerText,
        //                                                        CH.SelectSingleNode(".//name").Attributes["By"].InnerText,
        //                                                        CH.SelectSingleNode(".//value").InnerText,
        //                                                        CH.SelectSingleNode(".//value").Attributes["By"].InnerText,
        //                                                        ex);
        //                    if (CurrentKonfig.MainCharacteristics.Contains(temp))
        //                    {
        //                        mainFlaf = true;
        //                        continue;
        //                    }

        //                    else
        //                        ListCharectCat.Add(temp);
        //                    j++;
        //                }

        //                TempSubCatecory.Add(new SubCatecory(
        //                                                    SC.SelectSingleNode(".//name_our_categori").InnerText,
        //                                                    SC.SelectSingleNode(".//PortalCategoryId").InnerText,
        //                                                    SC.SelectSingleNode(".//discount").InnerText,
        //                                                    SC.SelectSingleNode(".//price").InnerText,
        //                                                    SC.SelectSingleNode(".//price_processing").InnerText,
        //                                                    SC.SelectSingleNode(".//combination").InnerText,
        //                                                    SC.SelectSingleNode(".//vendor").InnerText,
        //                                                    Convert.ToBoolean(SC.SelectSingleNode(".//combination").Attributes["value"].InnerText),
        //                                                    mainFlaf,
        //                                                    ListCharectCat));
        //            }
        //            CurrentCategory.Add(new Category(CurrentCategory.Count, Cat.SelectSingleNode(".//url").InnerText,
        //                                                        new List<SubCatecory>()));
        //            CurrentKonfig.Categores.Add(new Category(i, Cat.SelectSingleNode(".//url").InnerText,
        //                                                        TempSubCatecory));
        //            i++;
        //        }
        //    }

        //    CurrentKonfig.ShopName = shop;
        //    Session["kon"] = CurrentKonfig;
        //    Session["konLow"] = CurrentCategory;
        //    doc.Load(Server.MapPath("/") + "/Users/" + userId + @"\categories.xml");
        //    List<Categories> Categories = new List<Categories>();

        //    if (Session["Categories"] as List<Categories> != null) Categories = Session["Categories"] as List<Categories>;
        //    else
        //    {
        //        foreach (XmlNode Category in doc.SelectNodes("/categories/category"))
        //        {
        //            string parentId = "";
        //            if (Category.Attributes["parentId"] != null) parentId = Category.Attributes["parentId"].InnerText;
        //            bool flag = false;
        //            if (Categories.FirstOrDefault(x => x.name.Equals(Category.InnerText)) != null)
        //                flag = true;
        //            Categories.Add(new Categories(
        //                                            Category.InnerText,
        //                                            Category.Attributes["id"].InnerText,
        //                                            parentId,
        //                                            flag));
        //        }
        //        Session["Categories"] = Categories;
        //    }


        //    Session["kon"] = CurrentKonfig;
        //    Category NC = new Category();
        //    NC.id = CurrentKonfig.Categores.Count.ToString();
        //    NC.SubCatecories = new List<SubCatecory>();
        //    List<Characteristic> TC = new List<Characteristic>();
        //    TC.Add(new Characteristic("0", false, "", "", "", "", "", "", new List<Except>(1)));
        //    NC.SubCatecories.Add(new SubCatecory("", "", "", "", "", "", "", false, false, new List<Characteristic>(TC)));
        //    Session["NC"] = NC;
        //    ViewBag.NewCategory = NC;
        //    string[] t = new[] { "all", "low", "high" };

        //    SelectList PT = new SelectList(new[] { "all", "low", "high" });
        //    ViewBag.PriceType = PT;
        //    ViewBag.MainOptions = CurrentKonfig.MainOptions;
        //    ViewBag.MainCharacteristics = CurrentKonfig.MainCharacteristics;
        //    return View(CurrentCategory);
        //}

        //public void Save_changes(MainOption op)
        //{
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    var a = CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals(op.name));
        //    if (a != null)
        //    {
        //        CurrentKonfig.MainOptions[CurrentKonfig.MainOptions.IndexOf(a)] = op;
        //        Session["kon"] = CurrentKonfig;
        //    }
        //    else
        //    {

        //    }

        //}


        //public ActionResult Details(string id)
        //{
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    string[] t = new[] { "all", "low", "high" };

        //    SelectList PT = new SelectList(t);
        //    ViewBag.PriceType = PT;
        //    return PartialView(CurrentKonfig.Categores[Convert.ToInt32(id)]);
        //}

        //public ActionResult AutocompleteSearch(string term)
        //{
        //    List<Categories> Categories = new List<Categories>();
        //    Categories = Session["Categories"] as List<Categories>;
        //    var models = Categories.Where(a => a.name.Contains(term)).Select(a => new { value = a.name });

        //    return Json(models, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult SaveCategoryChange(Category ob)
        //{
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    var temp = CurrentKonfig.Categores.FirstOrDefault(x => x.id.Equals(ob.id));
        //    CurrentKonfig.Categores[CurrentKonfig.Categores.IndexOf(temp)] = ob;
        //    Session["kon"] = CurrentKonfig;

        //    return PartialView("request", "Файл сохранен");
        //}
        //public ActionResult AddCategory(Category ob)
        //{
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    CurrentKonfig.Categores.Add(ob);
        //    Session["kon"] = CurrentKonfig;
        //    Category NC = new Category();
        //    NC.id = CurrentKonfig.Categores.Count.ToString();
        //    NC.SubCatecories = new List<SubCatecory>();
        //    List<Characteristic> TC = new List<Characteristic>();
        //    TC.Add(new Characteristic("0", false, "", "", "", "", "", "", new List<Except>(1)));
        //    NC.SubCatecories.Add(new SubCatecory("", "", "", "", "", "", "", false, false, new List<Characteristic>(TC)));
        //    ViewBag.NewCategory = NC;

        //    string[] t = new[] { "all", "low", "high" };
        //    SelectList PT = new SelectList(t);
        //    ViewBag.PriceType = PT;
        //    ViewBag.MainOptions = CurrentKonfig.MainOptions;
        //    ViewBag.MainCharacteristics = CurrentKonfig.MainCharacteristics;
        //    return View("Index", CurrentKonfig.Categores.ToPagedList(1, 10));
        //}

        [HttpPost]
        public ActionResult SaveMainCharacteristicChange(Characteristic ob)
        {
            Konfig CurrentKonfig = new Konfig();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //var temp = db.Characteristics.FirstOrDefault(x => x.Id.Equals(ob.Id));
                db.Entry(ob).State = EntityState.Modified;
                foreach (var a in ob.Exceptions) db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
            }
            return PartialView("request", "Общая характеристика обновлена");
        }


        public ActionResult AddMainCharacteristic(string id)
        {
            Konfig CurrentKonfig = new Konfig();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurrentKonfig = db.KonfigVe.FirstOrDefault(x => x.Id.Equals(id));
                CurrentKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurrentKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
                Characteristic a = new Characteristic(false);

                CurrentKonfig.MainCharacteristics.Add(a);
                foreach (var ex in a.Exceptions) db.Entry(ex).State = EntityState.Added;
                db.Entry(a).State = EntityState.Added;
                db.Entry(CurrentKonfig).State = EntityState.Modified;
                db.SaveChanges();
                CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(id))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

                foreach (var SCat in CurrentKonfig.Categores)
                {
                    foreach (var SC in SCat.SubCatecories)
                    {
                        foreach (var Ch in SC.Characteristics)
                        {
                            foreach (var Ex in Ch.Exceptions) { }
                        }
                    }
                }
                CurrentKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurrentKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
            }
            return View("Index", CurrentKonfig);
        }
        public ActionResult DelMainCharacteristic(string id)
        {
            Konfig CurrentKonfig = new Konfig();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {



                Characteristic ch = db.Characteristics.Where(x => x.Id.Equals(id)).Include(x => x.Exceptions).First();
                string konId = ch.Konfig.Id;
                foreach (var a in ch.Exceptions) db.Entry(a).State = EntityState.Deleted;

                db.Entry(ch).State = EntityState.Deleted;
                db.SaveChanges();

                CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(konId))
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
                    .Include(x => x.Categores.Select(y => y.SubCatecories))
                    .First();

                foreach (var SCat in CurrentKonfig.Categores)
                {
                    foreach (var SC in SCat.SubCatecories)
                    {
                        foreach (var Ch in SC.Characteristics)
                        {
                            foreach (var Ex in Ch.Exceptions) { }
                        }
                    }
                }
                CurrentKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurrentKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();
            }
            return View("Index", CurrentKonfig);
        }
        public ActionResult AddSubCatecory(string ob)
        {
            Konfig CurrentKonfig = new Konfig();
            CurrentKonfig = Session["kon"] as Konfig;
            CurrentKonfig.Categores[Convert.ToInt32(ob)].SubCatecories.Add(new SubCatecory("", "", "", "", "", "", "", false, false, new List<Characteristic>(1)));
            string[] t = new[] { "all", "low", "high" };

            SelectList PT = new SelectList(t);
            ViewBag.PriceType = PT;
            Session["kon"] = CurrentKonfig;
            return PartialView("Details", CurrentKonfig.Categores[Convert.ToInt32(ob)]);
        }


        public ActionResult RemoveSubCatecory(string id, string idSub)
        {
            Konfig CurrentKonfig = new Konfig();
            CurrentKonfig = Session["kon"] as Konfig;
            CurrentKonfig.Categores[Convert.ToInt32(id)].SubCatecories.Remove(CurrentKonfig.Categores[Convert.ToInt32(id)].SubCatecories[Convert.ToInt32(idSub)]);
            string[] t = new[] { "all", "low", "high" };

            SelectList PT = new SelectList(t);
            ViewBag.PriceType = PT;
            Session["kon"] = CurrentKonfig;
            return PartialView("Details", CurrentKonfig.Categores[Convert.ToInt32(id)]);
        }


        public ActionResult AddSubCatecoryNew(string ob)
        {
            Category NC = new Category();
            NC = Session["NC"] as Category;
            NC.SubCatecories.Add(new SubCatecory("", "", "", "", "", "", "", false, false, new List<Characteristic>(1)));
            string[] t = new[] { "all", "low", "high" };

            SelectList PT = new SelectList(t);
            ViewBag.PriceType = PT;
            Session["NC"] = NC;
            return PartialView("AddCategory", NC);
        }


        public ActionResult RemoveSubCatecoryNew(string idSub)
        {
            Category NC = new Category();
            NC = Session["NC"] as Category;
            NC.SubCatecories.Remove(NC.SubCatecories[Convert.ToInt32(idSub)]);
            string[] t = new[] { "all", "low", "high" };

            SelectList PT = new SelectList(t);
            ViewBag.PriceType = PT;
            Session["NC"] = NC;
            return PartialView("AddCategory", NC);
        }


        //public ActionResult AddCharacteristic(string id, string SubId, string count)
        //{
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    CurrentKonfig.Categores[Convert.ToInt32(id)].SubCatecories[Convert.ToInt32(SubId)].Characteristics.Add(new Characteristic(count, false, "", "", "", "", "", "", new List<Except>(1)));
        //    Session["kon"] = CurrentKonfig;
        //    string[] t = new[] { "all", "low", "high" };

        //    SelectList PT = new SelectList(t);
        //    ViewBag.PriceType = PT;
        //    return View("Details", CurrentKonfig.Categores[Convert.ToInt32(id)]);
        //}


        //public ActionResult AddCharacteristicNew(string SubId, string count)
        //{
        //    Category NC = new Category();
        //    NC = Session["NC"] as Category;
        //    NC.SubCatecories[Convert.ToInt32(SubId)].Characteristics.Add(new Characteristic(count, false, "", "", "", "", "", "", new List<Except>(1)));
        //    Session["NC"] = NC;
        //    string[] t = new[] { "all", "low", "high" };

        //    SelectList PT = new SelectList(t);
        //    ViewBag.PriceType = PT;
        //    return View("AddCategory", NC);
        //}



        //public ActionResult SaveToFile()
        //{
        //    var userId = User.Identity.GetUserId();
        //    Konfig CurrentKonfig = new Konfig();
        //    CurrentKonfig = Session["kon"] as Konfig;
        //    DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);
        //    string konfig = "<shop>\r\n";
        //    foreach (var a in CurrentKonfig.MainOptions)
        //    {
        //        konfig += String.Format("<main_options name=\"{0}\" By=\"{1}\" take=\"{2}\">{3}</main_options>\r\n", a.name, a.By, a.take, a.value);
        //    }


        //    foreach (var a in CurrentKonfig.MainCharacteristics)
        //    {
        //        string te = "";
        //        foreach (var ex in a.Exceptions)
        //        {
        //            te += String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        //        }
        //        konfig += String.Format("<characteristics group=\"{0}\">\r\n", a.Group.ToString()) +
        //            String.Format("<main By=\"{0}\">{1}</main>\r\n", a.MainBy, a.Main) +
        //            String.Format("<name By=\"{0}\">{1}</name>\r\n", a.NameBy, a.Name) +
        //            String.Format("<value By=\"{0}\">{1}</value>\r\n", a.ValueBy, a.Value) +
        //            te +
        //            "</characteristics>\r\n";
        //    }
        //    konfig += "<categories>\r\n";

        //    foreach (var a in CurrentKonfig.Categores)
        //    {
        //        string SB = "";
        //        foreach (var Sb in a.SubCatecories)
        //        {
        //            string charac = "";
        //            foreach (var ch in Sb.Characteristics)
        //            {
        //                string te = "";
        //                foreach (var ex in ch.Exceptions)
        //                {
        //                    te += String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        //                }
        //                charac += String.Format("<characteristics group=\"{0}\">\r\n", ch.Group.ToString()) +
        //                    String.Format("<main By=\"{0}\">{1}</main>\r\n", ch.MainBy, ch.Main) +
        //                    String.Format("<name By=\"{0}\">{1}</name>\r\n", ch.NameBy, ch.Name) +
        //                    String.Format("<value By=\"{0}\">{1}</value>\r\n", ch.ValueBy, ch.Value) +
        //                    te +
        //                    "</characteristics>\r\n";
        //            }
        //            if (Sb.EnableMainCharacteristics)
        //            {
        //                foreach (var MC in CurrentKonfig.MainCharacteristics) charac += MC;
        //            }

        //            SB += Sb.ToString(Sb, CurrentKonfig.MainCharacteristics);
        //        }
        //        konfig += "<category>\r\n" +
        //                String.Format("<url>{0}</url>\r\n", a.url) +
        //                SB +
        //                "</category>\r\n";

        //    }
        //    konfig += "</categories>\r\n";
        //    konfig += "</shop>\r\n";
        //    FileInfo Konfig = new FileInfo(user.FullName + @"\" + CurrentKonfig.ShopName + @"\Konfig.xml");
        //    if (Konfig.Exists != false)
        //    {

        //        FileStream fs = Konfig.OpenWrite();
        //        Byte[] info = new UTF8Encoding(true).GetBytes(konfig);
        //        fs.Write(info, 0, info.Length);
        //        fs.Close();
        //    }
        //    return PartialView("request", "Файл сохранен");
        //}

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload, string Id)
        {
            Konfig CurrentKonfig = new Konfig();

            var userId = User.Identity.GetUserId();
            string fileName = System.IO.Path.GetFileName(upload.FileName);

            try
            {
                upload.SaveAs(@"C:\TempUsersFile\" + userId + fileName);
            }
            catch
            {
                ViewBag.req = String.Format("Ошибка при попытке скачивания файла");
                return View();
            }

            bool res = await req(@"C:\TempUsersFile\" + userId + fileName, Id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var a = db.SubCatecory;
                //CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(Id))
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
                //    .Include(x => x.VendorSearch)
                //    .Include(x => x.Contacts)
                //    .Include(x => x.Categores.Select(y => y.SubCatecories))
                //    .First();

                //foreach (var SCat in CurrentKonfig.Categores)
                //{
                //    foreach (var SC in SCat.SubCatecories)
                //    {
                //        foreach (var Ch in SC.Characteristics)
                //        {
                //            foreach (var Ex in Ch.Exceptions) { }
                //        }
                //    }
                //}

                CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(Id))
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
                if (CurrentKonfig.Contacts.Count == 0)
                {
                    CurrentKonfig.Contacts.Add(new Contacts() { Konfig = CurrentKonfig, Id = Guid.NewGuid().ToString() });
                }
                db.Entry(CurrentKonfig).Collection(x=> x.MainCharacteristics);
                foreach (var SCat in CurrentKonfig.Categores)
                {
                    foreach (var SC in SCat.SubCatecories)
                    {
                        db.Entry(SC).Collection("Characteristics");
                        foreach (var Ch in SC.Characteristics)
                        {
                            db.Entry(Ch).Collection("Exceptions");
                        }
                    }
                }
                foreach (var MCH in CurrentKonfig.MainCharacteristics)
                {
                    db.Entry(MCH).Collection("Exceptions");
                }
            }

            int SCatCount = 0;
            foreach (Category a in CurrentKonfig.Categores)
            {
                SCatCount += a.SubCatecories.Count;
            }
            if (res)
            {
                System.IO.File.Delete(@"C:\TempUsersFile\" + userId + fileName);
                ViewBag.req = String.Format("Файл спаршен, было найдено {0} категорий по {1} ссылкам", SCatCount, CurrentKonfig.Categores.Count());
                return View();
            }
            else
            {
                System.IO.File.Delete(@"C:\TempUsersFile\" + userId + fileName);
                ViewBag.req = String.Format("Ошибка в процессе разбора файла пожалуйста проверьте корректность данных в файле");
                return View();
            }
        }


        public Task<bool> req(string fileName, string Id)
        {

            Konfig CurrentKonfig = null;

            //CurrentKonfig.Categores = new List<Category>();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(Id))
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

                if (CurrentKonfig.Contacts.Count == 0)
                {
                    CurrentKonfig.Contacts.Add(new Contacts() { Konfig = CurrentKonfig, Id = Guid.NewGuid().ToString() });
                }
                
                Thread.Sleep(1);
                foreach (var SCat in CurrentKonfig.Categores)
                {
                    foreach (var SC in SCat.SubCatecories)
                    {
                        db.Entry(SC).Collection("Characteristics");
                        db.Entry(SC).Collection("KeyWord");
                        foreach (var Ch in SC.Characteristics)
                        {
                            db.Entry(Ch).Collection("Exceptions");
                            db.ExceptCH.RemoveRange(Ch.Exceptions);
                        }
                        db.Characteristics.RemoveRange(SC.Characteristics);
                        db.KeyWords.RemoveRange(SC.KeyWord);
                    }
                    db.SubCatecory.RemoveRange(SCat.SubCatecories);
                    
                }
                db.CategoryKonfig.RemoveRange(CurrentKonfig.Categores);

                
                db.SaveChanges();
            }
            if (fileName != null)
            {
                FileInfo newFile = new FileInfo(fileName);
                ExcelPackage p = new ExcelPackage(newFile);
                ExcelWorksheet ws = p.Workbook.Worksheets[1];

                
                int cell = 1;

                while (ws.Cells[1, cell].Text != "")
                {
                    cell++;
                }

                int row = 2;
                while (ws.Cells[row, 1].Text != "")
                {
                    string Url = ws.Cells[row, 1].Text;
                    string CategoryName = ws.Cells[row, 2].Text;
                    string PortalCategoryId = ws.Cells[row, 3].Text;
                    string Discount = ws.Cells[row, 4].Text;
                    string fee = ws.Cells[row, 5].Text;
                    string ShipingInUSA = ws.Cells[row, 6].Text;
                    string ShipingToUkr = ws.Cells[row, 7].Text;
                    string MoneyTransfer = ws.Cells[row, 8].Text;
                    string CustomTaxes = ws.Cells[row, 9].Text;
                    string PriceProcessing = ws.Cells[row, 10].Text;
                    string NameFilter = ws.Cells[row, 11].Text;
                    bool NameFilterValue = (ws.Cells[row, 12].Text == "+") ? true : false;
                    string PriceFilter = ws.Cells[row, 13].Text;
                    bool MainCharacteristics = (ws.Cells[row, 14].Text == "+") ? true : false;
                    string Vendor = ws.Cells[row, 15].Text;
                    List<KeyWord> Keys = new List<KeyWord>();
                    if (!String.IsNullOrEmpty(ws.Cells[row, 16].Text))
                    {
                        List<string> Temp = new List<string>(ws.Cells[row, 16].Text.Split(','));
                        foreach(string key in Temp)
                        {
                            string Name = key.TrimStart();
                            if (String.IsNullOrEmpty(Name)) continue;
                            bool CheakVendor = (Name.Contains("|")) ? true : false;
                            bool StringFormat = (Name.Contains("{0}")) ? true : false;
                            Name = (CheakVendor) ? Name.Remove(0, 1) : Name;
                            Keys.Add(new KeyWord() { Id = Guid.NewGuid().ToString(), Name = Name, CheakVendor = CheakVendor ,StringFormat = StringFormat });
                        }
                    }
                    List<Characteristic> Ch = new List<Characteristic>();

                    for (int i = 17; i < cell; i++)
                    {
                        if (!String.IsNullOrEmpty(ws.Cells[row, i].Text))
                        {
                            Ch.Add(new Characteristic(false,"","", ws.Cells[1, i].Text,"", ws.Cells[row, i].Text,"",new List<ExceptCH>()));
                        }

                    }
                    Category MCat = null;
                    try
                    {
                        MCat = CurrentKonfig.Categores.FirstOrDefault(x => x.url.Equals(Url));
                    }
                    catch
                    {
                        MCat = null;
                    }
                    if (MCat != null)
                    {
                        CurrentKonfig.Categores[CurrentKonfig.Categores.IndexOf(MCat)].SubCatecories.Add(
                            new SubCatecory(CategoryName, PortalCategoryId, Discount, PriceFilter, PriceProcessing, NameFilter, Vendor, NameFilterValue, MainCharacteristics, Ch) { fee= fee, ShippingInUS = ShipingInUSA , ShippingToUkraine = ShipingToUkr, MoneyTransfer = MoneyTransfer , CustomTaxes = CustomTaxes ,KeyWord= Keys });
                    }
                    else
                    {
                        SubCatecory SC = new SubCatecory(CategoryName, PortalCategoryId, Discount, PriceFilter, PriceProcessing, NameFilter, Vendor, NameFilterValue, MainCharacteristics, Ch) { fee = fee, ShippingInUS = ShipingInUSA, ShippingToUkraine = ShipingToUkr, MoneyTransfer = MoneyTransfer, CustomTaxes = CustomTaxes, KeyWord = Keys };
                        CurrentKonfig.Categores.Add(new Category(0, Url, new List<SubCatecory> { SC }));
                    }
                    row++;

                }
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Konfig temp = db.KonfigVe.First(x => x.Id.Equals(CurrentKonfig.Id));
                    temp.Categores = CurrentKonfig.Categores;

                    db.SaveChanges();
                }

                ws = null;
                p = null;

                return Task.Run(() =>
                    {
                        return true;
                    });
            }
            else
            {
                return Task.Run(() =>
                {
                    return false;
                });
            }


        }
          
        public void ProductPars(string shop , string userId)
        {
            

            Konfig CurKonfig = new Konfig();
            WebClient webClient = new WebClient();
            
            CurKonfig = db.KonfigVe.AsNoTracking().Where(x => x.ShopName.Equals(shop) & x.UserId.Equals(userId))
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
            
            List<ProductPars> PP = db.ProductsPars.Where(x=> x.ShopCode.Equals(CurKonfig.Id)).Include(x => x.Pictures).Include(x => x.Parameters).ToList();
            
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
            

            foreach(XmlNode node in doc.DocumentElement.SelectNodes("/yml_catalog/shop/offers/offer"))
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
                    var sds= node.SelectSingleNode("./url").InnerText;
                    sds ="" ;
                }
                string urlMP = node.SelectSingleNode("./picture").InnerText;
                string format = urlMP.Substring(urlMP.LastIndexOf("."));
                try
                {
                    webClient.DownloadFile(urlMP, PicPath + product.VendorCode + format);
                    if(product.Pictures.Count == 0) product.Pictures.Add(new Models.Picture("http://sshop.com.ua/Users/" + userId + "/" + CurKonfig.ShopName + "/Photos/" + product.VendorCode + format));
                }
                catch
                {

                }

                if (product == null) {
                    product.Id = Guid.NewGuid().ToString();
                    product.VendorCode = CurKonfig.VendorCode + new string('0', 5 - count.ToString().Length) + count.ToString();
                    PP.Add(product);
                }
            }
            foreach(var cat in CurKonfig.Categores)
            {
                var Prod = PP.Where(x => x.OurCategori.Equals(cat.url)).ToList();
                foreach(var s in cat.SubCatecories)
                {
                    double k = Convert.ToDouble(s.price_processing);
                    foreach (var p in Prod)
                    {
                        p.Relevance = true;
                        try
                        {
                            var a = p.PriceOld.Remove(p.PriceOld.IndexOf("."));
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
            }
            db.SaveChanges();
        }
        
    }
}