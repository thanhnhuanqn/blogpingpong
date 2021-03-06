﻿using System;
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
    public class TagsController : Controller
    {
        private const string Type = "tag";


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
                Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == Type).OrderByDescending(t=>t.Id).ToList()
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
            category.Taxonomy = Type;
            category.Description = form.Description;

            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);

            Database.Session.Save(category);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var category = Database.Session.Load<Term>((long)id);

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
            var category = Database.Session.Load<Term>((long)id);

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

            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);

            Database.Session.Update(category);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        
        private static void Delete(long id)
        {
            var category = Database.Session.Load<Term>(id);

            if (category == null) return ;

            Database.Session.Delete(category);
            Database.Session.Flush();           

        }


        /// <summary>
        /// Xoa cac bai viet co id chua trong listItem
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteTags()
        {
            var listIdPost = Request["DeleteTags"];

            if (string.IsNullOrEmpty(listIdPost)) return RedirectToAction("Index");

            var arraySlug = listIdPost.Split(',').Where(p => p != string.Empty).Distinct().ToArray();

            if (!arraySlug.Any()) return RedirectToAction("Index");

            foreach (var idPost in arraySlug)
            {
                long id;

                var flag = long.TryParse(idPost, out id);

                if (!flag) continue;

                Delete(id);
            }

            return RedirectToAction("Index");
        }
    }
}