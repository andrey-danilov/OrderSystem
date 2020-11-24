using Microsoft.AspNet.Identity;
using PagedList;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Data.Entity;
namespace report.Controllers
{
    public class AutoCorrectController : Controller
    {
        public XmlDocument doc = new XmlDocument();

        // GET: AutoCorrect
        public ActionResult Index(string shop)
        {
            var userId = User.Identity.GetUserId();
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurrentAutoCorrect = db.AutoCorrectList.Where(x => x.ShopName.Equals(shop) & x.UserId.Equals(userId)).ToList();
            }

            ViewBag.Name = shop;            
            return View(CurrentAutoCorrect);
        }


        [HttpPost]
        public ActionResult Index(AutoCorrect ob)
        {
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            CurrentAutoCorrect = Session["AutoCorrect"] as List<AutoCorrect>;

            List<AutoCorrect> req = Session["AutoCorrect"] as List<AutoCorrect>;

            if (!String.IsNullOrEmpty(ob.sku)) req = req.Where(x => x.sku.Contains(ob.sku)).ToList();
            if (!String.IsNullOrEmpty(ob.name)) req = req.Where(x => x.name.Contains(ob.name)).ToList();
            if (!String.IsNullOrEmpty(ob.description)) req = req.Where(x => x.description.Contains(ob.description)).ToList();
            if (!String.IsNullOrEmpty(ob.group)) req = req.Where(x => x.group.Contains(ob.group)).ToList();
            Session["AutoCorrectFindReq"] = req;
            Session["Filter"] = ob;
            ViewBag.Filter = ob;
            
            return View("Index", req);
        }

        [HttpPost]
        public ActionResult ClearFilter()
        {
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            CurrentAutoCorrect = Session["AutoCorrect"] as List<AutoCorrect>;
            Session["AutoCorrectFindReq"] = null;
            Session["Filter"] = new AutoCorrect();
            ViewBag.Filter = new AutoCorrect();
            int pageSize = 1;
            int pageNumber = 1;
            return View("Index", CurrentAutoCorrect.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SaveChange(AutoCorrect ob)
        {
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Entry(ob).State = EntityState.Modified;
                db.SaveChanges();
            }
            return PartialView("AutoCorrectDetails", ob);
        }


        public ActionResult Delete(string id)
        {
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            CurrentAutoCorrect = Session["AutoCorrect"] as List<AutoCorrect>;
            CurrentAutoCorrect.Remove(CurrentAutoCorrect[Convert.ToInt32(id)]);
            if (Session["Filter"] != null) ViewBag.Filter = Session["Filter"] as AutoCorrect;
            else ViewBag.Filter = new AutoCorrect();
            Session["AutoCorrect"] = CurrentAutoCorrect;
            return View("Index", CurrentAutoCorrect.ToPagedList(1, 10));
        }



        public ActionResult SaveToFile()
        {
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            CurrentAutoCorrect = Session["AutoCorrect"] as List<AutoCorrect>;

            var userId = User.Identity.GetUserId();


            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);
            FileInfo Konfig = new FileInfo(user.FullName + @"\" + Session["ShopName"] as string + @"\AutoCorrect.xml");
            string AutoCorrect = "<products>\r\n";

            foreach (var AU in CurrentAutoCorrect)
            {
                AutoCorrect += String.Format("<product id=\"{0}\">\r\n", AU.sku) +
                              String.Format("<name metod=\"{0}\" start=\"{1}\" end=\"{2}\">{3}</name>\r\n", AU.MetodsName, AU.nameStartIndex, AU.nameEndIndex, AU.name) +
                              String.Format("<description metod=\"{0}\" start=\"{1}\" end=\"{2}\">{3}</description>\r\n", AU.MetodsDescription, AU.descriptionStartIndex, AU.descriptionEndIndex, AU.description) +
                              String.Format("<group>{0}</group>\r\n</product>\r\n", AU.group);
            }
            AutoCorrect += "</products>\r\n";
            if (Konfig.Exists != false)
            {

                FileStream fs = Konfig.Create();
                Byte[] info = new UTF8Encoding(true).GetBytes(AutoCorrect);
                fs.Write(info, 0, info.Length);
                fs.Close();
            }
            return PartialView("request", "Файл сохранен");
        }


        public ActionResult AddItem(AutoCorrect ob)
        {
            var userId = User.Identity.GetUserId();
            List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ob.UserId = userId;
                db.Entry(ob).State = EntityState.Added;
                db.SaveChanges();
                CurrentAutoCorrect = db.AutoCorrectList.Where(x => x.ShopName.Equals(ob.ShopName) & x.UserId.Equals(ob.ShopName)).ToList();
            }

                ViewBag.Filter = new AutoCorrect();
            
            return View("Index",CurrentAutoCorrect);
        }

    }
}