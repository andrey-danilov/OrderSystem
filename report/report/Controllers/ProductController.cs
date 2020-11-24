using Microsoft.AspNet.Identity;
using PagedList;
using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace report.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Product
        public ActionResult Index(string shop, string relevance, int? page)
        {
            var userId = User.Identity.GetUserId();
            List<ProductPars> products = new List<ProductPars>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bool flag = Convert.ToBoolean(relevance);
                string id = db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(shop) & x.UserId.Equals(userId)).Id;
                products = db.ProductsPars.Where(x => x.ShopCode.Equals(id) & x.Relevance == flag).Include(x => x.Pictures).Include(x => x.Parameters).ToList();

                var AU = db.AutoCorrectList.Where(x=> x.UserId.Equals(userId) & x.ShopName.Equals(shop)).ToList();                
                foreach (ProductPars PPR in products)
                {                    
                    foreach (var a in AU.Where(x => !String.IsNullOrEmpty(x.MetodsDescription)))
                    {
                        switch (a.MetodsDescription)
                        {
                            case "RegexReplace":
                                if (!String.IsNullOrEmpty(PPR.Description) && Regex.IsMatch(a.descriptionStartIndex, PPR.Description))
                                {
                                    Regex regex = new Regex(a.descriptionStartIndex);
                                    PPR.Description = regex.Replace(PPR.Description, a.description);
                                }
                                break;
                            case "Replace":
                                if (!String.IsNullOrEmpty(PPR.Description) && PPR.Description.IndexOf(a.descriptionStartIndex) > -1)
                                {                                    
                                    PPR.Description = PPR.Description.Remove(PPR.Description.IndexOf(a.descriptionStartIndex), a.descriptionStartIndex.Length);
                                }
                                break;
                        }
                    }
                    foreach (var a in AU.Where(x => !String.IsNullOrEmpty(x.MetodsName)))
                    {
                        switch (a.MetodsName)
                        {
                            case "RegexReplace":
                                if (!String.IsNullOrEmpty(PPR.Denomination) && Regex.IsMatch(a.nameStartIndex, PPR.Denomination))
                                {
                                    Regex regex = new Regex(a.nameStartIndex);
                                    PPR.Denomination = regex.Replace(PPR.Denomination, a.description);
                                }
                                break;
                            case "Replace":
                                if (!String.IsNullOrEmpty(PPR.Denomination) && PPR.Denomination.IndexOf(a.nameStartIndex) > -1)
                                {     
                                                                  
                                    PPR.Denomination = PPR.Denomination.Remove(PPR.Denomination.IndexOf(a.nameStartIndex), a.nameStartIndex.Length);
                                }
                                break;
                            case "RemoveFrom":
                                if (!String.IsNullOrEmpty(PPR.Denomination) && PPR.Denomination.IndexOf(a.nameStartIndex) > -1)
                                {
                                    PPR.Denomination = PPR.Denomination.Remove(PPR.Denomination.IndexOf(a.nameStartIndex));
                                }
                                break;
                        }
                    }
                }
                db.SaveChanges();
            }
            int pageSize = 500;
            int pageNumber = (page ?? 1);
            ViewBag.NameShop = shop;
            ViewBag.relevance = relevance;
            
            return View(products.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult EdidRelevance(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ProductPars product = db.ProductsPars.FirstOrDefault(x => x.Id.Equals(id));
                product.Relevance = !product.Relevance;
                db.SaveChanges();
            }
           return PartialView("");
        }

        [HttpPost]
        public ActionResult Filter(ProductPars od, string shop, string relevance, int? page)
        {
            var userId = User.Identity.GetUserId();
            List<ProductPars> products = new List<ProductPars>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bool flag = Convert.ToBoolean(relevance);
                string id = db.KonfigVe.FirstOrDefault(x => x.ShopName.Equals(shop) & x.UserId.Equals(userId)).Id;
                products = db.ProductsPars.Where(x => x.ShopCode.Equals(id) & x.Relevance == flag).Include(x => x.Pictures).Include(x => x.Parameters).ToList();

            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.NameShop = shop;
            ViewBag.relevance = relevance;
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        
        public async Task<bool> Grouping()
        {
            //db.ProductsPars.RemoveRange(db.ProductsPars.Where(x => !x.ShopCode.Equals("fe254bb5-69a3-4dac-9820-86986bc8dbe3")).Include(x => x.Pictures).ToList());
            //db.SaveChanges();
            List<ProductPars> ProdList = await db.ProductsPars.Where(x => x.ShopCode.Equals("fe254bb5-69a3-4dac-9820-86986bc8dbe3") & x.Relevance & !x.VendorCode.Contains("_")).Include(x => x.Parameters).Include(x => x.Pictures).ToListAsync();
            List<ProductPars> ProdListAll = await db.ProductsPars.Where(x => x.ShopCode.Equals("fe254bb5-69a3-4dac-9820-86986bc8dbe3")).Include(x => x.Parameters).Include(x => x.Pictures).ToListAsync();
            foreach (ProductPars PP in ProdList)
            {
                foreach(Parameters Par in PP.Parameters.Where(x=>x.Name.Equals("Размеры:")))
                {
                    Par.Group = true;
                    db.Entry(Par).State = EntityState.Modified;
                }
                db.SaveChanges();
                List<Parameters> Sizes = PP.Parameters.Where(x => x.Group).OrderBy(x=> x.Value).ToList();
                List<Parameters> Static = PP.Parameters.Where(x => !x.Group).ToList();
                
                List<ProductPars> GProd = new List<ProductPars>();
                if(!String.IsNullOrEmpty(PP.GroupSKU))
                {
                    GProd = ProdListAll.Where(x => x.GroupSKU.Equals(PP.VendorCode)).ToList();
                }
                if(Sizes.Count > 0)
                {
                    
                    List<ProductPars> NewSizes = new List<ProductPars>();
                    for (int i = 0; i < Sizes.GetRange(0, Sizes.Count - 1).Count; i++)
                    {
                        PP.Parameters.Remove(Sizes[i]);
                        List<ProductPars> Intersect = new List<ProductPars>();
                        if (GProd.Count > 0)
                        {
                            Intersect = Sizes[i].ProductsPars.Intersect(GProd).ToList();
                        }
                        
                        if (Intersect.ToList().Count == 1)
                        {
                            ProductPars Temp = Intersect[0];
                            Temp.Pictures.Clear();
                            db.Entry(Temp).State = EntityState.Modified;
                            Temp.Pictures = PP.Pictures;
                            Temp.Price = PP.Price;
                            Temp.PriceOld = PP.PriceOld;
                            Temp.Description = PP.Description;
                            db.Entry(Temp).State = EntityState.Modified;
                            GProd.Remove(Intersect[0]);
                        }
                        else
                        {
                            
                            ProductPars Temp = new ProductPars(PP.UserId, PP.ShopCode , PP.VendorCode + "_" + (i+1).ToString(), PP.OurCategori, PP.PortalCategoryId, PP.Vendor, PP.Price,
                                                                PP.PriceOld, PP.Url, PP.Denomination, PP.Description, PP.KeyWords, PP.Pictures, new List<Parameters>());

                            Temp.Id = Guid.NewGuid().ToString();
                            Temp.GroupSKU = PP.VendorCode;
                            Temp.Relevance = true;
                            Temp.Parameters.AddRange(Static);
                            Temp.Parameters.Add(Sizes[i]);
                            NewSizes.Add(Temp);
                        }
                    }
                    if (NewSizes.Count > 0)
                    {
                        db.ProductsPars.AddRange(NewSizes);
                        db.SaveChanges();
                    }

                    
                    PP.GroupSKU = PP.VendorCode;
                    db.Entry(PP).State = EntityState.Modified;
                    if (GProd.Count > 0)
                    {
                        foreach (ProductPars GPP in GProd)
                        {
                            db.Pictures.RemoveRange(GPP.Pictures);
                            db.Parameterses.RemoveRange(GPP.Parameters);
                        }
                        db.ProductsPars.RemoveRange(GProd);
                    }
                    db.SaveChanges();
                }
                
            }



            return true;
            
        }


        public ActionResult Dublicate()
        {
            
            var D = db.ProductsPars.Where(x=> x.Relevance).Include(x => x.Parameters).Include(x => x.Pictures).ToList().GroupBy(x=> x.VendorCode);
            int C = 1;
            foreach(var Du in D)
            {
                if(Du.Count()>1)
                {
                    foreach(var CurD in Du.ToList().GetRange(0,Du.Count()-1))
                    {
                        CurD.VendorCode = CurD.VendorCode.Remove(2) + new String('0', (7- C.ToString().Count())) + C.ToString();
                        db.Entry(CurD).State = EntityState.Modified;
                        C=C+1;
                    }
                }
                
            }
            db.SaveChanges();
            return View();
        }


        public ActionResult Photos()
        {
            var ProdL = db.ProductsPars.Where(x => x.ShopCode.Equals("fe254bb5-69a3-4dac-9820-86986bc8dbe3")).Include(x => x.Pictures).Include(x => x.Parameters).ToList();
            

            foreach(var prod in ProdL)
            {
                
                Picture[] ProdPic = new Picture[prod.Pictures.Count];
                prod.Pictures.CopyTo(ProdPic);
                foreach(var pic in ProdPic)
                {
                    try
                    {
                        string Pname = pic.url.Substring(pic.url.LastIndexOf("/") + 1);
                        string pUrl = pic.url.Remove(pic.url.LastIndexOf("/") + 1);

                        Bitmap bmp = new Bitmap(Image.FromFile(@"C:\inetpub\wwwroot\Users\eb2f3d5d-c4d9-4d46-a837-418142ed9f54\6pm\Photos\" + Pname));
                        bmp.GetThumbnailImage(bmp.Width / 4, bmp.Height / 4, null, IntPtr.Zero).Save(@"C:\inetpub\wwwroot\Users\eb2f3d5d-c4d9-4d46-a837-418142ed9f54\6pm\Photos\" + "M" + Pname);
                        bmp.GetThumbnailImage(bmp.Width / 10, bmp.Height / 10, null, IntPtr.Zero).Save(@"C:\inetpub\wwwroot\Users\eb2f3d5d-c4d9-4d46-a837-418142ed9f54\6pm\Photos\" + "S" + Pname);
                    }
                    catch
                    {

                    }

                    
                    //prod.Pictures.Add(new Picture("http://sshop.com.ua/Users/eb2f3d5d-c4d9-4d46-a837-418142ed9f54/6pm/Photos/" + "M" + Pname));
                    //prod.Pictures.Add(new Picture("http://sshop.com.ua/Users/eb2f3d5d-c4d9-4d46-a837-418142ed9f54/6pm/Photos/" + "S" + Pname));
                    //db.Entry(prod).State = EntityState.Modified;
                }
            }
            //db.SaveChanges();
            
            return View();
        }



    }
}