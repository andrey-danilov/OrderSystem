using Microsoft.AspNet.Identity;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.Threading;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Runtime.InteropServices;
using PagedList;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Opera;
using System.Net;
using report.Services;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace report.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Orders
        public ActionResult Index(int? page)
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();
            ApplicationUser user = new ApplicationUser();
            SearchOrderProm search = new SearchOrderProm();
            OrderProm req = new OrderProm();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).OrderByDescending(x => x.number).ToList();

                foreach (var o in orders)
                {
                    o.Statuses = o.Statuses.OrderBy(x => x.number).ToList();
                    o.Notes = o.Notes.OrderBy(x => x.number).ToList();
                }
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();

                req.Products = new List<ProductProm>();

                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    req.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }

                ViewBag.OrderReq = req;
                if (Session["OrderSearch"] as SearchOrderProm == null)
                {
                    ViewBag.OrderSearch = search;
                    Session["OrderSearch"] = search;
                }
                else
                {
                    search = Session["OrderSearch"] as SearchOrderProm;
                    ViewBag.OrderSearch = search;
                }


                if (Session["OrderSearchList"] as SearchOrderProm == null)
                {
                    
                    List<UserStatus> temp_L = db.UserStatuses.AsNoTracking().Where(x => x.ApplicationUser.Id.Equals(userId)).ToList();
                    temp_L = temp_L.OrderBy(x => x.number).ToList();
                    var Statuses = new List<Status>();
                    foreach (UserStatus t in temp_L)
                    {
                        Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                    }
                    ViewBag.OrderSearchList = Statuses;
                    Session["OrderSearchList"] = Statuses;
                }
                else
                {
                    ViewBag.OrderSearchList = Session["OrderSearchList"] as List<Status>;
                }
                //if (db.OP.FirstOrDefault(x => x.Id.Equals(userId)) != null)
                //{
                //OrderProm a = db.OP.Where(x => x.Id.Equals(userId)).Include(x => x.Statuses).First();
                //a.Statuses = a.Statuses.OrderBy(x => x.number).ToList();
                //    ViewBag.OrderSearchList = a.Statuses;
                //    search.DeliveryInfo = a.DeliveryInfo;
                //    search.FirstData = new DateTime(2018, 01, 01, 00, 00, 00);
                //    search.SecondtData = new DateTime(2018, 01, 01, 00, 00, 00);
                //    search.TotalPrice = a.TotalPrice;
                //    search.Phone = a.Phone;
                //    search.Order_Number = search.Order_Number;
                //    search.WaybillNumber = search.WaybillNumber;
                //}
                //else
                //{
                //    OrderProm a = new OrderProm();

                //    a.Id = userId;
                //    a.Statuses = new List<Status>();

                //    List<UserStatus> temp_L = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UStatuses).First().UStatuses.ToList();
                //    temp_L = temp_L.OrderBy(x => x.number).ToList();
                //    foreach (UserStatus t in temp_L)
                //    {
                //        a.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                //    }
                //    db.OP.Add(a);
                //    db.SaveChanges();
                //    ViewBag.OrderSearchList = a.Statuses;
                //}



                int pageSize = 50;
                int pageNumber = (page ?? 1);
                return View(orders.ToPagedList(pageNumber, pageSize));
            }
        }


        public ActionResult IndexImposed(int? page)
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();
            ApplicationUser user = new ApplicationUser();
            SearchOrderProm search = new SearchOrderProm();
            OrderProm req = new OrderProm();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).OrderByDescending(x => x.number).ToList();
                orders = orders.Where(x => x.Statuses.FirstOrDefault(y => y.StatusName.Contains("Наложенный") & y.flag) != null).ToList();
                foreach (var o in orders)
                {
                    o.Statuses = o.Statuses.OrderBy(x => x.number).ToList();
                    o.Notes = o.Notes.OrderBy(x => x.number).ToList();
                }
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();

                req.Products = new List<ProductProm>();

                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    req.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }

                ViewBag.OrderReq = req;
                if (Session["OrderSearch"] as SearchOrderProm == null)
                {
                    ViewBag.OrderSearch = search;
                    Session["OrderSearch"] = search;
                }
                else
                {
                    search = Session["OrderSearch"] as SearchOrderProm;
                    ViewBag.OrderSearch = search;
                }


                if (Session["OrderSearchList"] as SearchOrderProm == null)
                {
                    List<UserStatus> temp_L = db.UserStatuses.AsNoTracking().Where(x => x.ApplicationUser.Id.Equals(userId)).ToList();
                    temp_L = temp_L.OrderBy(x => x.number).ToList();
                    var Statuses = new List<Status>();
                    foreach (UserStatus t in temp_L)
                    {
                        Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                    }
                    ViewBag.OrderSearchList = Statuses;
                    Session["OrderSearchList"] = Statuses;
                }
                else
                {
                    ViewBag.OrderSearchList = Session["OrderSearchList"] as List<Status>;
                }
                //if (db.OP.FirstOrDefault(x => x.Id.Equals(userId)) != null)
                //{
                //OrderProm a = db.OP.Where(x => x.Id.Equals(userId)).Include(x => x.Statuses).First();
                //a.Statuses = a.Statuses.OrderBy(x => x.number).ToList();
                //    ViewBag.OrderSearchList = a.Statuses;
                //    search.DeliveryInfo = a.DeliveryInfo;
                //    search.FirstData = new DateTime(2018, 01, 01, 00, 00, 00);
                //    search.SecondtData = new DateTime(2018, 01, 01, 00, 00, 00);
                //    search.TotalPrice = a.TotalPrice;
                //    search.Phone = a.Phone;
                //    search.Order_Number = search.Order_Number;
                //    search.WaybillNumber = search.WaybillNumber;
                //}
                //else
                //{
                //    OrderProm a = new OrderProm();

                //    a.Id = userId;
                //    a.Statuses = new List<Status>();

                //    List<UserStatus> temp_L = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UStatuses).First().UStatuses.ToList();
                //    temp_L = temp_L.OrderBy(x => x.number).ToList();
                //    foreach (UserStatus t in temp_L)
                //    {
                //        a.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                //    }
                //    db.OP.Add(a);
                //    db.SaveChanges();
                //    ViewBag.OrderSearchList = a.Statuses;
                //}



                int pageSize = orders.Count;
                int pageNumber = (page ?? 1);
                return View("Index",orders.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult IndexControl(int? page)
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();
            ApplicationUser user = new ApplicationUser();
            SearchOrderProm search = new SearchOrderProm();
            OrderProm req = new OrderProm();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).OrderByDescending(x => x.number).ToList();
                orders = orders.Where(x => x.Statuses.FirstOrDefault(y => y.StatusName.Contains("Контроль") & y.flag) != null).ToList();
                foreach (var o in orders)
                {
                    o.Statuses = o.Statuses.OrderBy(x => x.number).ToList();
                    o.Notes = o.Notes.OrderBy(x => x.number).ToList();
                }
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();

                req.Products = new List<ProductProm>();

                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    req.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }

                ViewBag.OrderReq = req;
                if (Session["OrderSearch"] as SearchOrderProm == null)
                {
                    ViewBag.OrderSearch = search;
                    Session["OrderSearch"] = search;
                }
                else
                {
                    search = Session["OrderSearch"] as SearchOrderProm;
                    ViewBag.OrderSearch = search;
                }


                if (Session["OrderSearchList"] as SearchOrderProm == null)
                {
                    List<UserStatus> temp_L = db.UserStatuses.AsNoTracking().Where(x => x.ApplicationUser.Id.Equals(userId)).ToList();
                    temp_L = temp_L.OrderBy(x => x.number).ToList();
                    var Statuses = new List<Status>();
                    foreach (UserStatus t in temp_L)
                    {
                        Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                    }
                    ViewBag.OrderSearchList = Statuses;
                    Session["OrderSearchList"] = Statuses;
                }
                else
                {
                    ViewBag.OrderSearchList = Session["OrderSearchList"] as List<Status>;
                }
                



                int pageSize = orders.Count;
                int pageNumber = (page ?? 1);
                return View("Index", orders.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Search(SearchOrderProm OP)
        {
            var userId = User.Identity.GetUserId();
            var Orders = await db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Notes).Include(x => x.Products).Include(x => x.Statuses).ToListAsync();
            if(!String.IsNullOrEmpty( OP.Order_Number))
            {
                Orders = Orders.Where(x => x.Order_Number.Contains(OP.Order_Number)).ToList();
            }
            if (!String.IsNullOrEmpty(OP.DeliveryInfo))
            {
                Orders = Orders.Where(x => x.DeliveryInfo.ToUpper().Contains(OP.DeliveryInfo.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(OP.Phone))
            {
                Orders = Orders.Where(x => x.Phone.Contains(OP.Phone)).ToList();
            }
            if (!String.IsNullOrEmpty(OP.TotalPrice))
            {
                Orders = Orders.Where(x => x.TotalPrice.Equals(OP.TotalPrice)).ToList();
            }
            if (!String.IsNullOrEmpty(OP.ShopNumber) & !OP.ShopNumber.Equals("x-"))
            {
                Orders = Orders.Where(x => x.ShopNumber != null).ToList();
                Orders = Orders.Where(x => x.ShopNumber.Equals(OP.ShopNumber)).ToList();
            }
            try
            {
                if (!String.IsNullOrEmpty(OP.WaybillNumber))
                {
                    Orders = Orders.Where(x => x.WaybillNumber != null).ToList();
                    Orders = Orders.Where(x => x.WaybillNumber.Contains(OP.WaybillNumber)).ToList();
                }
            }
            catch
            {

            }
            
            if (!String.IsNullOrEmpty(OP.Note))
            {
                Orders = Orders.Where(x => x.Notes.Where(y=> y.NoteName.Contains(OP.Note)).Count() > 0).ToList();
            }
            if (!String.IsNullOrEmpty(OP.ProductName))
            {
                Orders = Orders.Where(x => x.Products.Where(y => y.Name.Contains(OP.ProductName)).Count() > 0).ToList();
            }
            if (!String.IsNullOrEmpty(OP.ProductPrice))
            {
                Orders = Orders.Where(x => x.Products.Where(y => y.Price.Equals(OP.ProductPrice)).Count()>0).ToList();
            }
            
            if(Orders.Count>0)
            {
                return PartialView("ListOrder", Orders.OrderByDescending(x => x.number).ToPagedList(1, Orders.Count));

            }
            else
            {
                return PartialView("request", "Таких ордеров нет");
            }
        }


        public ActionResult Add_States(string id)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var orders = db.OP.Where(x => x.Id_User.Equals(userId) & x.Order_Number.Equals(id)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).OrderByDescending(x => x.number).ToList();
                var USa =db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UStatuses).First().UStatuses.Count;
                orders = orders.Where(x => x.Statuses.Count < 10).ToList();
                List<UserStatus> US = db.UserStatuses.AsNoTracking().Where(x => x.ApplicationUser.Id.Equals(userId)).ToList();
                foreach (var or in orders)
                {
                    db.Statuses.RemoveRange(or.Statuses);
                    db.SaveChanges();
                    
                    foreach (UserStatus t in US)
                    {
                        or.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                    }
                    db.SaveChanges();
                }
            }

            return View();
        }


        public ActionResult Edid_state(string id)
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();
            OrderProm order = new OrderProm();
            ApplicationUser user = new ApplicationUser();
            List<Status> OrderSearchList = Session["OrderSearchList"] as List<Status>;
            ViewBag.OrderSearchList = Session["OrderSearchList"] as List<Status>;
            OrderSearchList.FirstOrDefault(x => x.Id.Equals(id)).flag = !(OrderSearchList.FirstOrDefault(x => x.Id.Equals(id)).flag);
            Session["OrderSearchList"] = OrderSearchList;
            return PartialView(OrderSearchList.FirstOrDefault(x => x.Id.Equals(id)));
        }

        

        public ActionResult Find_Con(string id)
        {
            List<Contacts> Cont = new List<Contacts>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Cont = db.KonfigVe.Where(x => x.VendorCode.Equals(id.Remove(2))).Include(x=> x.Contacts).FirstOrDefault().Contacts;
            }

            return PartialView(Cont);
        }


        public ActionResult Edid_state_s(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //SendStatus(id);
                Status Stat = db.Statuses.FirstOrDefault(x => x.Id.Equals(id));
                Stat.flag = !(Stat.flag);
                db.SaveChanges();
                return PartialView(Stat);
            }
            
        }

        public ActionResult Edid_DoneState_s(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Status Stat = db.Statuses.FirstOrDefault(x => x.Id.Equals(id));
                Stat.flag = !(Stat.flag);
                db.SaveChanges();
                //SendStatus(id);
                var OPL = db.OP.Where(x => x.Id != null).Include(x => x.Statuses).ToList();

                string Ids = OPL.FirstOrDefault(x => x.Statuses.Contains(Stat)).Order_Number;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://my.prom.ua/api/v1/orders/set_status");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer 94c0536c63b1dca0d6dbe44c236fa710e55809bd");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{ \"ids\": [" + Ids + "]  ,  \"status\": \"delivered\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                return PartialView("Edid_state_s", Stat);
            }

        }

        public ActionResult Edid_CanselState_s(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Status Stat = db.Statuses.FirstOrDefault(x => x.Id.Equals(id));
                Stat.flag = !(Stat.flag);
                db.SaveChanges();
                var OPL = db.OP.Where(x => x.Id != null).Include(x => x.Statuses).ToList();

                string Ids = OPL.FirstOrDefault(x => x.Statuses.Contains(Stat)).Order_Number;
                //SendStatus(id);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://my.prom.ua/api/v1/orders/set_status");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer 94c0536c63b1dca0d6dbe44c236fa710e55809bd");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{  \"status\": \"canceled\",  \"ids\": [" + Ids + "],  \"cancellation_reason\": \"not_available\",  \"cancellation_text\": \"\" }";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                return PartialView("Edid_state_s", Stat);
            }

        }

        public void SendStatus(string id)
        {
            var Status = db.Statuses.FirstOrDefault(x => x.Id.Equals(id));
            var Orders = db.OP.Where(x=> x.FromShop).Include(x => x.Statuses).ToList();
            var Order = Orders.FirstOrDefault(x => x.Statuses.Contains(Status));
            string url = String.Format("http://localhost:57423/Orders/EditStatus?id={0}&StatusName={1}", Order.Id, Status.StatusName);
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

        public ActionResult EdidWaybillNumber(string id , string WaybillNumber)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                OrderProm OP = db.OP.FirstOrDefault(x => x.Id.Equals(id));
                db.Entry(OP).Collection(x => x.Statuses).Load();
                db.Entry(OP).Collection(x => x.Notes).Load();
                db.Entry(OP).Collection(x => x.Products).Load();
                OP.WaybillNumber = WaybillNumber;

                //string url = String.Format("http://localhost:57423/Orders/AddTrackingNumber?id={0}&TrackingNumber={1}", id,WaybillNumber);
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Method = "GET";
                //request.Accept = "application/json";
                //request.UserAgent = "Mozilla/5.0 ....";

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //StreamReader reader = new StreamReader(response.GetResponseStream());
                //StringBuilder output = new StringBuilder();
                //output.Append(reader.ReadToEnd());
                //response.Close();

                db.SaveChanges();
                return PartialView("Save_changes", OP);
            }

        }
        public void EditShopNumber(string id, string ShopNumber)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                OrderProm OP = db.OP.FirstOrDefault(x => x.Id.Equals(id));
                OP.ShopNumber = ShopNumber;
                db.SaveChanges();
            }
        }

        


        [HttpPost]
        public ActionResult Staus_search()
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();            
            List<Status> OrderSearchList = Session["OrderSearchList"] as List<Status>;
            OrderProm search = new OrderProm();
            bool triger = true;
            foreach (Status s in OrderSearchList)
            {
                if (s.flag) triger = false;
            }
            if (triger)
            {
                return PartialView("request", "Ошибка поиска, отсуцтвие критериев поиска");
            }
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Statuses).Include(x => x.Products).Include(x => x.Notes).ToList();
                     
                    foreach (var o in orders)
                    {
                        o.Statuses = o.Statuses.OrderBy(x => x.number).ToList();
                    }

                    foreach (Status s in OrderSearchList)
                    {
                        if (s.flag) orders = orders.Where(x => x.Statuses.Where(y => y.number.Equals(s.number) & y.flag).ToList().Count != 0).ToList();
                    }
                }
                orders = orders.OrderByDescending(x => x.number).ToList();
                int pageSize = orders.Count;
                int pageNumber = 1;
                //ViewBag.OrderSearch = search;
                if (orders.Count > 0)
                {
                    return PartialView("ListOrder", orders.ToPagedList(pageNumber, pageSize));

                }
                else
                {
                    return PartialView("request", "Таких товаров нет");
                }
            }
            catch
            {
                return PartialView("request", "Ошибка поиска");
            }

        }

        public ActionResult Edid_note(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Note a = db.Notes.FirstOrDefault(x => x.Id.Equals(id));
                a.flag = !(a.flag);
                db.SaveChanges();
                return PartialView(a);
            }
        }

        public ActionResult Edid_product(ProductProm promProduct)
        {
            var userId = User.Identity.GetUserId();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                db.Entry(promProduct).State = EntityState.Modified;
                db.SaveChanges();


                return PartialView(promProduct);
            }




        }


        public ActionResult Add_input()
        {

            return PartialView("Add_product", new ProductProm());
        }

        public ActionResult ProductList(int? page)
        {
            Product search = new Product();
            var userId = User.Identity.GetUserId();
            List<Product> req = new List<Product>();
            OrderProm r = new OrderProm();
            r.Products = new List<ProductProm>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.Products.FirstOrDefault(x => x.Id.Equals(userId)) == null)
                {
                    search.Id = userId;
                    search.ProductParameters = new List<Parameters>();

                    search.ProductParameters.Add(new Parameters());
                    search.ProductParameters.Add(new Parameters());
                    search.ProductParameters.Add(new Parameters());

                    db.Products.Add(search);
                    db.SaveChanges();

                }
                search = db.Products.Where(x => x.Id.Equals(userId)).Include(x => x.ProductParameters).ToList().First();
                if (page != null)
                {
                    List<Product> products = db.Products.ToList();
                    products.Remove(products.FirstOrDefault(x => x.Id.Equals(userId)));
                    if (!String.IsNullOrEmpty(search.vendorCode)) products = products.Where(x => x.vendorCode.Contains(search.vendorCode)).ToList();
                    if (!String.IsNullOrEmpty(search.Name)) products = products.Where(x => x.Name.Contains(search.Name)).ToList();
                    if (!String.IsNullOrEmpty(search.KeyWorsd)) products = products.Where(x => x.KeyWorsd.Contains(search.KeyWorsd)).ToList();
                    if (!String.IsNullOrEmpty(search.vendor)) products = products.Where(x => x.vendor.Contains(search.vendor)).ToList();
                    foreach (var P in search.ProductParameters)
                    {
                        if (!String.IsNullOrEmpty(P.Name) && !String.IsNullOrEmpty(P.Value)) products = products.Where(x => x.ProductParameters.Where(y => y.Name.Equals(P.Name) && y.Name.Equals(P.Value)).ToList().Count != 0).ToList();
                    }
                    foreach (var a in products)
                    {
                        req.Add(new Product(a.vendorCode, a.Url, a.Price, a.Name, a.Id));
                    }



                }
                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    r.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }
                ViewBag.OrderReq = r;
                ViewBag.SProduct = search;
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(req.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpPost]
        public ActionResult ProductList(Product p, int? page, int? i)
        {
            OrderProm r = new OrderProm();
            r.Products = new List<ProductProm>();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                p.Id = userId;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                List<Product> products = db.Products.Include(x => x.ProductParameters).ToList();
                products.Remove(products.FirstOrDefault(x => x.Id.Equals(userId)));
                if (!String.IsNullOrEmpty(p.vendorCode)) products = products.Where(x => x.vendorCode.Contains(p.vendorCode)).ToList();
                if (!String.IsNullOrEmpty(p.Name)) products = products.Where(x => x.Name.Contains(p.Name)).ToList();
                if (!String.IsNullOrEmpty(p.KeyWorsd)) products = products.Where(x => x.KeyWorsd.Contains(p.KeyWorsd)).ToList();
                if (!String.IsNullOrEmpty(p.vendor)) products = products.Where(x => x.vendor.Contains(p.vendor)).ToList();

                foreach (var P in p.ProductParameters)
                {
                    if (!String.IsNullOrEmpty(P.Name) && !String.IsNullOrEmpty(P.Value)) products = products.Where(x => x.ProductParameters.Where(y => y.Name.Equals(P.Name) && y.Name.Equals(P.Value)).ToList().Count != 0).ToList();
                }
                List<Product> req = new List<Product>();
                foreach (var a in products)
                {
                    req.Add(new Product(a.vendorCode, a.Url, a.Price, a.Name, a.Id));
                }

                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    r.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }
                ViewBag.OrderReq = r;
                ViewBag.SProduct = p;
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(req.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult CreatePV()
        {
            var userId = User.Identity.GetUserId();
            OrderProm req = new OrderProm();
            req.Products = new List<ProductProm>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {
                    req.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", ""));
                }
            }
            ViewBag.OrderReq = req;
            return PartialView(req);
        }

        [HttpPost]
        public ActionResult CreatePV(OrderProm O)
        {
            var userId = User.Identity.GetUserId();
            List<OrderProm> orders = new List<OrderProm>();
            OrderProm req = new OrderProm();
            req.Products = new List<ProductProm>();
            OrderProm search = new OrderProm();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                short temp = db.OP.Where(x => x.Id_User.Equals(userId)).OrderBy(x => x.number).ToList().Last().number;
                temp++;
                O.number = temp;
                O.Id = Guid.NewGuid().ToString();
                O.Id_User = userId;
                O.Statuses = new List<Status>();
                O.Notes = new List<Note>();
                foreach (UserStatus S in db.UserStatuses.AsNoTracking().Where(x => x.ApplicationUser.Id.Equals(userId)).ToList())
                {
                    O.Statuses.Add(new Status(S.StatusName, S.DefoltFlag, S.number));
                }

                foreach (ProductProm p in O.Products)
                {
                    p.Id = Guid.NewGuid().ToString();
                    p.Price = db.Products.FirstOrDefault(x => x.vendorCode.Equals(p.VendorCode)).Price.ToString();
                    p.Url = db.Products.FirstOrDefault(x => x.vendorCode.Equals(p.VendorCode)).Url;
                }
                db.OP.Add(O);
                db.TempProductLists.RemoveRange(db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList());
                db.SaveChanges();


                orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).OrderByDescending(x => x.number).ToList();

                foreach (var o in orders)
                {
                    o.Statuses = o.Statuses.OrderBy(x => x.number).ToList();
                }

                ViewBag.OrderReq = req;
                search = db.OP.Where(x => x.Id.Equals(userId)).Include(x => x.Statuses).First();
                search.Statuses = search.Statuses.OrderBy(x => x.number).ToList();
                ViewBag.OrderSearch = search;
                return View("Index", orders);
            }

        }



        public ActionResult AddToTempList()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddToTempList(Product a)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var temp = db.Products.FirstOrDefault(x => x.Id.Equals(a.Id));
                db.TempProductLists.Add(new TempProductList(userId, temp.vendorCode, a.Quantity, temp.Name));
                db.SaveChanges();
                OrderProm req = new OrderProm();
                req.Products = new List<ProductProm>();
                List<TempProductList> te = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in te)
                {
                    req.Products.Add(new ProductProm(t.Id, t.VendorCode, "", t.Quantity.ToString(), "", t.Name));
                }

                ViewBag.OrderReq = req;
                return PartialView("CreatePV", req);
            }
        }

        public ActionResult DelFromTL(string id)
        {
            var userId = User.Identity.GetUserId();
            OrderProm req = new OrderProm();
            req.Products = new List<ProductProm>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                TempProductList t = db.TempProductLists.FirstOrDefault(x => x.Id.Equals(id));
                if (t != null)
                {
                    db.TempProductLists.Remove(t);
                    db.SaveChanges();
                    List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                    foreach (TempProductList te in temp)
                    {
                        req.Products.Add(new ProductProm(te.Id, te.VendorCode, "", te.Quantity.ToString(), "", te.Name));
                    }
                }
                ViewBag.OrderReq = req;

                return PartialView("CreatePV", req);
            }
        }

        public ViewResult Create()
        {
            OrderProm req = new OrderProm();
            req.Products = new List<ProductProm>();
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<TempProductList> temp = db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (TempProductList t in temp)
                {

                    req.Products.Add(new ProductProm(t.VendorCode, "", "", t.Quantity.ToString(), "", t.Name));
                }
            }



            return View("Create", req);
        }

        [HttpPost]
        public ActionResult Create(OrderProm O_P)
        {
            var userId = User.Identity.GetUserId();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                {

                    var T_last = db.OP.Where(x => x.Id_User.Equals(userId));

                    OrderProm temp = new OrderProm();

                    temp.Id = Guid.NewGuid().ToString();
                    temp.Id_User = userId;
                    if (T_last.Count() != 0)
                    {
                        int O_N = T_last.Count();
                        temp.Order_Number = (O_N++).ToString();
                    }
                    else
                    {
                        temp.Order_Number = "1";
                    }

                    temp.Data = O_P.Data;

                    temp.DeliveryInfo = O_P.DeliveryInfo;
                    temp.Phone = O_P.Phone;
                    foreach (var p in temp.Products)
                    {
                        p.Url = db.Products.FirstOrDefault(x => x.vendorCode.Equals(p.VendorCode)).Url;
                    }


                    foreach (var p in db.TempProductLists.Where(x => x.UserId.Equals(userId)).ToList())
                    {
                        db.TempProductLists.Remove(p);
                    }

                    db.OP.Add(temp);
                    db.SaveChanges();
                    return RedirectToAction("Details", temp);
                }
                else
                {
                    return View("Create");
                }
            }

        }

        public ActionResult Save_changes(string id)
        {
            var userId = User.Identity.GetUserId();
            OrderProm a = new OrderProm();
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                a = db.OP.FirstOrDefault(x => x.Id.Equals(id));
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();
                ViewBag.Stat = new SelectList(user.UStatuses, "Id", "StatusName");

                ViewBag.S = new Status();
                return PartialView(a);
            }
        }

        public ActionResult EdidOrder(string id)
        {
            var userId = User.Identity.GetUserId();
            OrderProm a = new OrderProm();
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                a = db.OP.FirstOrDefault(x => x.Id.Equals(id));
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();
                ViewBag.Stat = new SelectList(user.UStatuses, "Id", "StatusName");

                ViewBag.S = new Status();
                return PartialView(a);
            }
        }
        
        [HttpPost]
        public ActionResult Save_changes(OrderProm a)
        {
            var userId = User.Identity.GetUserId();
            OrderProm temp = new OrderProm();
            List<OrderProm> orders = new List<OrderProm>();
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                a.Id_User = userId;
                if (ModelState.IsValid)
                {
                    db.Entry(a).State = EntityState.Modified;
                    db.SaveChanges();
                }

                orders = db.OP.Where(x => x.Id_User.Equals(userId)).Include(x => x.Products).Include(x => x.Statuses).Include(x => x.Notes).ToList();
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();
            }
            a.Statuses = a.Statuses.OrderBy(x => x.number).ToList();
            ViewBag.Stat = user.UStatuses;
            ViewBag.nod = user.UNotes;

            return PartialView(a);
        }


        public ActionResult Add_status()
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                user = db.Users.Where(x => x.Id.Equals(userId)).Include(x => x.UNotes).Include(x => x.UStatuses).First();
            }
            ViewBag.Stat = new SelectList(user.UStatuses, "Id", "StatusName");
            ViewBag.S = new Status();
            return PartialView();
        }
        
        public ViewResult Details(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //ViewBag.Products = db.ProductsProm.ToList();
                var O_P = db.OP.Where(x => x.Id.Equals(id)).Include(x => x.Products).First();
                return View("Details", O_P);
            }

        }

        public ViewResult Add_Product(string id)
        {
            return View("Add_Product", new ProductProm());
        }

        public ActionResult Add_note(Note ob)
        {
            if (String.IsNullOrEmpty(ob.NoteName)) return HttpNotFound();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ob.flag = true;
                var op = db.OP.Where(x => x.Id.Equals(ob.Id)).Include(x=> x.Notes).First();                
                ob.Id = Guid.NewGuid().ToString();
                ob.number = ((short)(op.Notes.Count + 1));                
                op.Notes.Add(ob);
                db.Notes.Add(ob);

                db.SaveChanges();
                
                return PartialView("Edid_note", ob);
            }
                
        }

        public ActionResult SelectPhones()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var a = db.OP.AsNoTracking().Where(x => x.TrakingStatus.Equals(7)).Include(x => x.Statuses).ToList();
                a = a.Where(x=> x.Statuses.Where(y=> y.flag & y.StatusName.Equals("Контроль")).ToList().Count>0).ToList();
                List<string> phones = new List<string>();
                foreach(var p in a)
                {
                    phones.Add(p.Phone);
                }
                return PartialView("SelectPhones" , String.Join(";" , phones));
            }
        }

        public ActionResult SelectNotPaidPhones()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var a = db.OP.AsNoTracking().Include(x => x.Statuses).ToList();
                a = a.Where(x => x.Statuses.Where(y => y.flag & y.StatusName.Equals("Контроль") ).ToList().Count > 0).ToList();
                a = a.Where(x => x.Statuses.Where(y => y.flag & y.StatusName.Equals("Реквизиты")).ToList().Count > 0).ToList();
                a = a.Where(x => x.Statuses.Where(y => !y.flag & y.StatusName.Equals("Оплачен")).ToList().Count > 0).ToList();
                a = a.Where(x => x.Statuses.Where(y => !y.flag & y.StatusName.Equals("Выполнен")).ToList().Count > 0).ToList();

                List<string> phones = new List<string>();
                foreach (var p in a)
                {
                    phones.Add(p.Phone);
                }
                return PartialView("SelectPhones", String.Join(";", phones));
            }
        }

        [HttpPost]
        public ActionResult Add_Product(ProductProm Product)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                OrderProm Main = db.OP.FirstOrDefault(x => x.Id.Equals(Product.Id));
                Main.Products = new List<ProductProm>();
                Main.Products.Add(new ProductProm(Product.VendorCode, "", Product.Price, Product.Quantity, Product.Url, Product.Name));
                db.SaveChanges();
                return PartialView("Save_changes", Main);
            }
        }


        public ActionResult SendS(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Order = db.OP.FirstOrDefault(x => x.Order_Number.Equals(id));
                db.Entry(Order).Collection(x => x.Statuses).Load();
                db.Entry(Order).Collection(x => x.Notes).Load();
                db.Entry(Order).Collection(x => x.Products).Load();
                Order.Statuses.FirstOrDefault(x => x.StatusName.Equals("Выполнен")).flag = true;
                db.SaveChanges();
                var a = new SMSWorker();
                
                a.Auth("SshopGateway", "SshopGateway");
                
                var res = a.SendSMS("SportBay", Order.Phone, "Накладная №" +Order.WaybillNumber+ ".Спасибо за покупку! Магазин СпортБэй.", "");
                return PartialView("request", String.Join("|", res));
            }

        }
    }
}


