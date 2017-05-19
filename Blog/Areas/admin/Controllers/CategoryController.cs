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
    [Authorize(Roles = "admin")]
    [SelectedTab("Posts")]
    public class CategoryController : Controller
    {

        public bool CheckSlugUnique(long id, string slug)
        {
            var post = Database.Session.Query<Term>().Where(p => p.Slug == slug);

            if (id <= 0) return post.Any();

            post = Database.Session.Query<Term>().Where(p => p.Id != id && p.Slug == slug);

            return post.Any();

        }

        // GET: admin/Category
        public ActionResult Index()
        {
            return View(new CategoryIndex
            {
                Category = Database.Session.Query<Term>().Where(t=>t.Taxonomy == "cat").ToList()
            });
        }
        public ActionResult New()
        {
            return View(new CategoryNew
            {
                Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList()
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
                form.Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();
                return View(form);
            }

            category.Name = form.Name;
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Taxonomy = "cat";
            category.Description = form.Description;
            category.Parent = form.Parent ;
            category.Count = 0;
            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);
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
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                Parent = category.Parent,
                Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == "cat").ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryNew form)
        {            
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == "cat").ToList();
                return View(form);
            }

            category.Name = form.Name;
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Description = form.Description;
            category.Parent = form.Parent;
            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);

            Database.Session.Update(category);            
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            var category = Database.Session.Load<Term>(id);

            if (category == null) return HttpNotFound();
            
            Database.Session.Delete(category);
            Database.Session.Flush();

            var searchParent = Database.Session.Query<Term>().Where(t => t.Parent == id);

            foreach (var term in searchParent)
            {
                term.Parent = 0;
                Database.Session.Update(term);
                Database.Session.Flush();
            }
            
            return RedirectToAction("Index");

        }
    }
}