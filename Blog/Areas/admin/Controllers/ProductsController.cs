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
    [SelectedTab("Products")]
    public class ProductsController : Controller
    {
        private readonly string TagTypeProduct = "product_tag";
        private readonly string CategoryTypeProduct = "product_cat";
        private readonly string TypeProduct = "product";
        // GET: admin/Products
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            return View(new Product());
        }

        #region Categories


        public bool CheckSlugUnique(long id, string slug)
        {
            var post = Database.Session.Query<Term>().Where(p => p.Slug == slug);

            if (id <= 0) return post.Any();

            post = Database.Session.Query<Term>().Where(p => p.Id != id && p.Slug == slug);

            return post.Any();

        }
        public ActionResult Categories()
        {
            return View(new CategoryIndex
            {
                Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == CategoryTypeProduct).ToList()
            });
        }


        public ActionResult CategoryNew()
        {
            return View(new CategoryNew
            {
                Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == CategoryTypeProduct).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryNew(CategoryNew form)
        {
            var category = new Term();

            if (Database.Session.Query<Term>().Any(x => x.Slug == form.Slug))
            {
                ModelState.AddModelError("Slug", "Slug nay da co");
            }

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == CategoryTypeProduct).ToList();
                return View(form);
            }

            category.Name = form.Name;
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Taxonomy = CategoryTypeProduct;
            category.Description = form.Description;
            category.Parent = form.Parent;
            category.Count = 0;
            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);
            Database.Session.Save(category);
            Database.Session.Flush();

            return RedirectToAction("Categories");
        }

        public ActionResult CategoryEdit(int id)
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
                Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == CategoryTypeProduct).ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(int id, CategoryNew form)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == CategoryTypeProduct).ToList();
                return View(form);
            }

            category.Name = form.Name;
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Description = form.Description;
            category.Parent = form.Parent;
            category.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, category.Slug, category.Id);

            Database.Session.Update(category);
            Database.Session.Flush();

            return RedirectToAction("Categories");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryDelete(int id)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            Database.Session.Delete(category);
            Database.Session.Flush();
            return RedirectToAction("Categories");

        }

        #endregion Categories


        #region Tags

        public ActionResult Tags()
        {
            return View(new CategoryIndex
            {
                Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == TagTypeProduct).ToList()
            });
        }


        public ActionResult TagNew()
        {
            return View(new CategoryNew
            {
                Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == TagTypeProduct).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TagNew(CategoryNew form)
        {
            var category = new Term();

            if (Database.Session.Query<Term>().Any(x => x.Slug == form.Slug))
            {
                ModelState.AddModelError("Slug", "Slug nay da co");
            }

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == TagTypeProduct).ToList();
                return View(form);
            }

            category.Name = form.Name.Trim();
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Taxonomy = TagTypeProduct;
            category.Description = form.Description;

            Database.Session.Save(category);
            Database.Session.Flush();

            return RedirectToAction("Tags");
        }


        public ActionResult TagEdit(int id)
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
                Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == TagTypeProduct).ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TagEdit(int id, CategoryNew form)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                form.Categories = Database.Session.Query<Term>().Where(t => t.Id != id && t.Taxonomy == TagTypeProduct).ToList();
                return View(form);
            }

            category.Name = form.Name.Trim();
            category.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            category.Description = form.Description;
            category.Parent = form.Parent;

            Database.Session.Update(category);
            Database.Session.Flush();

            return RedirectToAction("Tags");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TagDelete(int id)
        {
            var category = Database.Session.Load<Term>((Int64)id);

            if (category == null) return HttpNotFound();

            Database.Session.Delete(category);
            Database.Session.Flush();
            return RedirectToAction("Tags");

        }

        #endregion Tags

        public ActionResult Attributes()
        {
            return View();
        }
    }
}