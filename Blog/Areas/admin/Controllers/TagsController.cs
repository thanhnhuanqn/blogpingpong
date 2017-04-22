using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;

namespace Blog.Areas.admin.Controllers
{
    [SelectedTab("Tag")]
    public class TagsController : Controller
    {
        private string Type = "tag";
        
        // GET: admin/Category
        public ActionResult Index()
        {
            return View(new CategoryIndex
            {
                Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == Type).ToList()
            });
        }
        public ActionResult New()
        {
            return View(new CategoryNew
            {
                Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == Type).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(CategoryNew form)
        {
            var category = new Term();

            if (Database.Session.Query<Term>().Any(x => x.Slug == form.Slug))
            {
                ModelState.AddModelError("Slug", "Slug nay da co");
            }

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == Type).ToList();
                return View(form);
            }

            category.Name = form.Name.Trim();
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Taxonomy = "tag";
            category.Description = form.Description;            

            Database.Session.Save(category);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            return View(new CategoryNew
            {
                Id = category.Id,
                Name = category.Name.Trim(),
                Slug = category.Slug,
                Description = category.Description,
                Parent = category.Parent,
                Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == Type).ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryNew form)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == Type).ToList();
                return View(form);
            }

            category.Name = form.Name.Trim();
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Description = form.Description;
            category.Parent = form.Parent;

            Database.Session.Update(category);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            Database.Session.Delete(category);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }
    }
}