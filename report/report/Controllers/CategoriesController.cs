using Microsoft.AspNet.Identity;
using report.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace report.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        public ActionResult Index()
        {
            XmlDocument doc = new XmlDocument();
            var userId = User.Identity.GetUserId();
            DirectoryInfo path = new DirectoryInfo(Server.MapPath("/") + "/Users/" + userId);
            
            List<ShopCategories> Categories = new List<ShopCategories>();                       
            using (ApplicationDbContext db = new ApplicationDbContext())
            {               
                Categories = db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList();
                db.ShopCategories.RemoveRange(Categories);
                Categories = new List<ShopCategories>();
                doc.Load(path + @"\categories.xml");
                foreach (XmlNode Category in doc.SelectNodes("/categories/category"))
                {

                    string parentId = "";
                    if (Category.Attributes["parentId"] != null) parentId = Category.Attributes["parentId"].InnerText;
                    bool flag = false;
                    if (Categories.FirstOrDefault(x => x.name.Equals(Category.InnerText)) != null)
                        flag = true;
                    Categories.Add(new ShopCategories(
                                                    Category.InnerText,
                                                    userId,
                                                    parentId,
                                                    Category.Attributes["id"].InnerText,
                                                    flag
                                                    ));
                }
                db.ShopCategories.AddRange(Categories);
                db.SaveChanges();
            }
            return View(Categories);
        }

        [HttpPost]
        public ActionResult EditCategory(ShopCategories ob)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.ShopCategories.FirstOrDefault(x => x.Id.Equals(ob.Id)) != null)
                {
                    db.Entry(ob).State = EntityState.Modified;
                    db.SaveChanges();
                    return PartialView("request", "Категория " + ob.name + " обновлена");
                }
                else
                {
                    ob.UserId = userId;
                    db.Entry(ob).State = EntityState.Added;
                    db.SaveChanges();
                    return View("request", "Категория " + ob.name + " добавлена");
                }                
            }            
        }

        [HttpPost]
        public ViewResult AddCategory(ShopCategories ob)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                    ob.UserId = userId;
                    db.Entry(ob).State = EntityState.Added;
                    db.SaveChanges();
                    return View("Index" ,db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList());                
            }
        }


        public ActionResult Delete(string ob)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ShopCategories SC = db.ShopCategories.FirstOrDefault(x => x.Id.Equals(ob));
                db.Entry(SC).State = EntityState.Deleted;
                db.SaveChanges();
                return View("Index" , db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList());
            }
        }
    }
}


