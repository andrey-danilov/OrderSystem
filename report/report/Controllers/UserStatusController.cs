using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using report.Models;
using Microsoft.AspNet.Identity;

namespace report.Controllers
{
    public class UserStatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserStatus
        public async Task<ActionResult> Index()
        {
            return View(await db.UserStatuses.OrderBy(x=> x.number).ToListAsync());
        }

        // GET: UserStatus/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStatus userStatus = await db.UserStatuses.FindAsync(id);
            if (userStatus == null)
            {
                return HttpNotFound();
            }
            return View(userStatus);
        }

        // GET: UserStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserStatus/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserStatus userStatus)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                userStatus.DefoltFlag = false;
                userStatus.Id = Guid.NewGuid().ToString();
                var US =  db.Users.Where(x => x.Id.Equals(userId)).Include(x=> x.UStatuses).First().UStatuses;
                userStatus.number = Convert.ToInt16(US.Count + 1);
                US.Add(userStatus);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(userStatus);
        }

        // GET: UserStatus/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStatus userStatus = await db.UserStatuses.FindAsync(id);
            if (userStatus == null)
            {
                return HttpNotFound();
            }
            return View(userStatus);
        }

        // POST: UserStatus/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserStatus userStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userStatus).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userStatus);
        }

        

        // POST: UserStatus/Delete/5
        
        
        public async Task<ActionResult> Delete(string id)
        {
            UserStatus userStatus = await db.UserStatuses.FindAsync(id);
            db.UserStatuses.Remove(userStatus);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
