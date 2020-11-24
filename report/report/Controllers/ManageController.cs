using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using report.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Principal;
using HtmlAgilityPack;
using System.IO;
using System.Data.Entity;
using System.Diagnostics;
using System.Net;

namespace report.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Настроен поставщик двухфакторной проверки подлинности."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : message == ManageMessageId.AddPhoneSuccess ? "Ваш номер телефона добавлен."
                : message == ManageMessageId.RemovePhoneSuccess ? "Ваш номер телефона удален."
                : "";

            var userId = User.Identity.GetUserId();
            
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                model.PromExports = db.PromExports.ToList();
            }

                return View(model);
        }



        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }


        public async Task<ActionResult> ParsProductsFromFiles()
        {
            bool a = await Pars();
            
            return PartialView();
        }

        public Task<bool> Pars()
        {
            var userId = User.Identity.GetUserId();
            List<string> folders = Directory.GetDirectories(Server.MapPath("~") + @"\Users\" + userId.ToString() + @"\").ToList();
            HtmlDocument products = new HtmlDocument();
            HtmlDocument products_list = new HtmlDocument(); using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser Us = db.Users.FirstOrDefault(x => x.Id.Equals(userId));
                Us.Products = new List<Product>();
                foreach (string fol in folders)
                {
                    products.Load(fol + @"\products.xml");
                    products_list.Load(fol + @"\products_list.xml");


                    Dictionary<string, string> Ursl = new Dictionary<string, string>();
                    foreach (HtmlNode product in products_list.DocumentNode.SelectNodes("product"))
                    {
                        Ursl.Add(product.Id, product.GetAttributeValue("url", string.Empty));
                    }

                    foreach (HtmlNode product in products.DocumentNode.SelectNodes("offer"))
                    {
                        string VD = product.GetAttributeValue("id", String.Empty);
                        string Name = "";
                        if (products.DocumentNode.SelectNodes(product.XPath + "/name") != null)
                        {
                            foreach (HtmlNode n in products.DocumentNode.SelectNodes(product.XPath + "/name"))
                            {
                                Name = n.SelectNodes(product.XPath + "/name").First().InnerText;
                            }
                        }
                        string price = "";
                        if (products.DocumentNode.SelectNodes(product.XPath + "/price") != null)
                        {
                            foreach (HtmlNode n in products.DocumentNode.SelectNodes(product.XPath + "/price"))
                            {
                                price = product.SelectNodes(product.XPath + "/price").First().InnerText;
                            }
                        }

                        string o_price = "";
                        if (products.DocumentNode.SelectNodes(product.XPath + "/oldprice") != null)
                        {
                            foreach (HtmlNode n in products.DocumentNode.SelectNodes(product.XPath + "/oldprice"))
                            {
                                o_price = product.SelectNodes(product.XPath + "/oldprice").First().InnerText;
                            }
                        }

                        string KeyWord = "";
                        if (products.DocumentNode.SelectNodes(product.XPath + "/keywords") != null)
                        {
                            foreach (HtmlNode n in products.DocumentNode.SelectNodes(product.XPath + "/keywords"))
                            {
                                KeyWord = product.SelectNodes(product.XPath + "/keywords").First().InnerText;
                            }
                        }


                        string vendor = "";
                        if(products.DocumentNode.SelectNodes(product.XPath + "/vendor") != null)
                        {
                            foreach (HtmlNode n in products.DocumentNode.SelectNodes(product.XPath + "/vendor"))
                            {
                                vendor = product.SelectNodes(product.XPath + "/vendor").First().InnerText;
                            }
                        }
                        

                        Dictionary<string, string> param = new Dictionary<string, string>();
                        if (products.DocumentNode.SelectNodes(product.XPath + "/p") != null)
                        {
                            foreach (HtmlNode p in products.DocumentNode.SelectNodes(product.XPath + "/p"))
                            {
                                try
                                {
                                    param.Add(p.SelectNodes(p.XPath + "/param").First().GetAttributeValue("name", String.Empty), p.InnerText);
                                }
                                catch { continue; }
                            }
                        }



                        Product t = new Product(VD, Ursl.FirstOrDefault(x => x.Key.Equals(VD)).Value, Int32.Parse(price), Int32.Parse(o_price), "", Name,KeyWord,vendor);
                        t.ProductParameters = new List<Parameters>();
                        foreach (var p in param)
                        {
                            t.ProductParameters.Add(new Parameters(p.Key, p.Value, false));
                        }
                        Us.Products.Add(t);

                        
                        db.SaveChanges();
                    }

                }
            }


            return Task.Run(() =>
            {
                return true;
            });
        }

        public async Task<bool> Run(string Url , HttpPostedFileBase files)
        {
            var userId = User.Identity.GetUserId();
            if (files != null && files.ContentLength > 0)
            {
                files.SaveAs(@"W:\TempUsersFile\" + userId + files.FileName);
            }

            string path = @"W:\TempUsersFile\" + userId + files.FileName;
            string AdvertisingFilePath = new DirectoryInfo(Server.MapPath("/") + @"/Users/" + userId).FullName;
            Process.Start(new DirectoryInfo(Server.MapPath("/")).FullName + @"Advertising/facebook.exe", Url +" "+ path + " " + AdvertisingFilePath);

            return true;
        }

        public RedirectResult AddPE(string Url)
        {
            var userId = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.Id.Equals(userId)).First();
                if (user.PromExports == null) user.PromExports = new List<PromExport>();
                user.PromExports.Add(new PromExport() { Id = Guid.NewGuid().ToString(), Url = Url });
                db.SaveChanges();
            }
            return Redirect("/Manage");
        }

        //public ViewResult AddPromData()
        //{
        //    return View("AddPromData", new AddPromDataViewModel());
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPromData(AddPromDataViewModel model)
        //{
        //    string currentUserId = User.Identity.GetUserId();
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        var user = db.Users.FirstOrDefault(x => x.Id.Equals(currentUserId));
        //        //user.PromLogin = model.Login;
        //        //user.PromPassword = model.Password;
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Index", "Manage");

        //}

        //
        // GET: /Manage/AddPhoneNumber
        public ViewResult AddPhoneNumber()
        {

            //return View();
            return View("AddPhoneNumber", new AddPhoneNumberViewModel());

        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            string currentUserId = User.Identity.GetUserId();
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                var user = db.Users.FirstOrDefault(x => x.Id.Equals(currentUserId));
                user.PhoneNumber = model.Number;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Manage");
            
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}