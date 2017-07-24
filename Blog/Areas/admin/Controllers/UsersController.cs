using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;

namespace Blog.Areas.admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("users")]
    public class UsersController : Controller
    {

        private void SyncRoles(IList<RoleCheckbox> checkboxes, IList<Role> roles)
        {
            var selectedRoles = new List<Role>();

            foreach (var role in Database.Session.Query<Role>())
            {
                var checkbox = checkboxes.Single(c => c.Id == role.Id);
                checkbox.Name = role.Name;

                if (checkbox.IsChecked)
                    selectedRoles.Add(role);
            }

            foreach (var toAdd in selectedRoles.Where(t => !roles.Contains(t)))
                roles.Add(toAdd);

            foreach (var toRemove in roles.Where(t => !selectedRoles.Contains(t)).ToList())
                roles.Remove(toRemove);
        }

        // GET: admin/User
        public ActionResult Index()
        {
            return View(new UsersIndex
            {
                Users = Database.Session.Query<User>().ToList()
            });
        }

        public ActionResult New()
        {
            return View(new UserNew
            {
                Roles = Database.Session.Query<Role>().Select(role => new RoleCheckbox
                {
                    Id = role.Id,
                    IsChecked = false,
                    Name = role.Name
                }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(UserNew form)
        {
            var user = new User();

            SyncRoles(form.Roles, user.Roles);
            
            if (Database.Session.Query<User>().Any(u => u.UserName == form.UserName))
            {
                ModelState.AddModelError("UserName", "UserName must be unique");
            }
            if (Database.Session.Query<User>().Any(u => u.Email == form.Email))
            {
                ModelState.AddModelError("Email", "This email has been registered");
            }
            if (!ModelState.IsValid)
            {
                return View(form);
            }            
            user.Email = form.Email.Trim();
            user.UserName = form.UserName.ToLower().Trim();
            user.SetPassword(form.Password);
            user.Url = form.Url;
            user.DisplayName = string.IsNullOrEmpty(form.DisplayName) ? user.UserName : form.DisplayName;
            user.ActivationKey = BCrypt.Net.BCrypt.GenerateSalt();

            Database.Session.Save(user);           
            Database.Session.Flush();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null) return HttpNotFound();

            return View(new UserEdit
            {
                UserName = user.UserName,
                Email = user.Email,
                Url =  user.Url,
                DisplayName =  user.DisplayName,
                Roles = Database.Session.Query<Role>().Select(role => new RoleCheckbox()
                {
                    Id = role.Id,
                    IsChecked = user.Roles.Contains(role),
                    Name = role.Name
                }).ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserEdit form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null) return HttpNotFound();

            SyncRoles(form.Roles, user.Roles);

            if (Database.Session.Query<User>().Any(u => u.UserName == form.UserName && u.Id != id))
            {
                ModelState.AddModelError("Username", "UserName must be unique");
            }
            if (!ModelState.IsValid) return View(form);

            user.UserName = form.UserName.ToLower().Trim();
            user.Email = form.Email.Trim();
            user.Url = form.Url;
            user.DisplayName = string.IsNullOrEmpty(form.DisplayName) ? user.UserName : form.DisplayName;

            Database.Session.Update(user);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null) return HttpNotFound();

            return View(new UsersResetPassword
            {
                UserName = user.UserName
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetPassword(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null) return HttpNotFound();

            if (!ModelState.IsValid) return View(form);

            user.SetPassword(form.Password);

            Database.Session.Update(user);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);

            if (user == null) return HttpNotFound();

            Database.Session.Delete(user);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }

    }
}