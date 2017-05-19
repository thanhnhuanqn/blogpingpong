using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Blog.Areas.admin.ViewModels;
using Blog.Models;
using NHibernate.Linq;

namespace Blog.Areas.admin.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auth");
        }
        public ActionResult Login()
        {
            return View(new AuthLogin() { });
        }
        [HttpPost]
        public ActionResult Login(AuthLogin form, string returnUrl)
        {

            var user = Database.Session.Query<User>().FirstOrDefault(u => u.UserName == form.UserName);

            if (user == null || !user.CheckPassword(form.Password))
            {
                ModelState.AddModelError("Username", "Username or password is incorrect");
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }


            FormsAuthentication.SetAuthCookie(user.UserName, true);
            
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Posts");
        }
    }
}