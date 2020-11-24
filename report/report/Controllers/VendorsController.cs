using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace report.Controllers
{
    public class VendorsController : Controller
    {
        // GET: Vendors
        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return View(db.VendorsNames.ToList());
            } 
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                VendorsName Vn = new VendorsName() { Id = Guid.NewGuid().ToString(), Name = name };
                db.VendorsNames.Add(Vn);
                db.SaveChanges();
                return View("Index", db.VendorsNames.ToList());
            }
        }

        
        public ActionResult Delete(string Id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.VendorsNames.Remove(db.VendorsNames.FirstOrDefault(x=> x.Id.Equals(Id)));
                db.SaveChanges();
                return View("Index", db.VendorsNames.ToList());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload)
        {
            string fileName = System.IO.Path.GetFileName(upload.FileName);
            var userId = User.Identity.GetUserId();
            try
            {
                upload.SaveAs(@"C:\TempUsersFile\" + userId + fileName);
                fileName = @"C:\TempUsersFile\" + userId + fileName;
            }
            catch
            {
                ViewBag.req = String.Format("Ошибка при попытке скачивания файла");
                return View();
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                
                if (fileName != null)
                {
                    FileInfo newFile = new FileInfo(fileName);
                    ExcelPackage p = new ExcelPackage(newFile);
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];


                    int cell = 1;
                    int Aded = 0;
                    while (ws.Cells[cell, 1].Text != "")
                    {
                        string ven = ws.Cells[cell, 1].Text;
                        if (db.VendorsNames.FirstOrDefault(x=> x.Name.ToUpper().Equals(ven.ToUpper())) ==null)
                        {
                            VendorsName Vn = new VendorsName() { Id = Guid.NewGuid().ToString(), Name = ven };
                            db.VendorsNames.Add(Vn);
                            Aded++;
                        }
                        
                        
                        cell++;
                    }
                    ViewBag.req = String.Format("Добавлено брендов: {0}", Aded);
                }
                db.SaveChanges();
                
                return View("Index", db.VendorsNames.ToList());
            }
        }
    }
}