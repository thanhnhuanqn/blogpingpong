using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;
using Blog.Module.Paging;

namespace Blog.Areas.admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("page")]
    public class PagesController : Controller
    {
        private string TypePost = "page";
        
        private const int DefaultPageSize = 15;

        public static bool CheckSlugUnique(long id, string slug)
        {
            var post = Database.Session.Query<Post>().Where(p => p.Slug == slug);

            if (id <= 0) return post.Any();

            post = Database.Session.Query<Post>().Where(p => p.Id != id && p.Slug == slug);

            return post.Any();

        }

        // GET: admin/Posts
        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(Database.Session.Query<Post>().Where(p => p.Type == TypePost).OrderByDescending(t => t.CreateAt).ToPagedList(currentPageIndex, DefaultPageSize));
        }

        public ActionResult New()
        {
            return View(new PostsForm
            {
                Day = DateTime.Now.Day,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                Hour = DateTime.Now.Hour,
                Minutes = DateTime.Now.Minute,
                Tags = null,
                Category = null
            });
        }
        

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(PostsForm form)
        {
                        
            if (!ModelState.IsValid)
                return View(new PostsForm
                {
                    Day = DateTime.Now.Day,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Hour = DateTime.Now.Hour,
                    Minutes = DateTime.Now.Minute,
                    Tags = Database.Session.Query<Term>().Where(t => t.Taxonomy == "tag").ToList(),
                    Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList()
                });
            

            var page = new Post
            {
                CreateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0),
                User = Auth.User,
                Title = form.Title,
                Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly(),
                Excerpt = form.Excerpt,
                Content = form.Content,
                Type = TypePost,
                Status = form.Status,
                CommentStatus = "open"
            };
            page.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, page.Slug, page.Id);

            Database.Session.Save(page);
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id)
        {
            //var post = Database.Session.Load<Post>((Int64)id);
            var page = Database.Session.Query<Post>().SingleOrDefault(t => t.Id == id);

            if (page == null) return HttpNotFound();
                        
            return View(new PostsForm
            {
                Id = id,
                Title = page.Title,
                Slug = page.Slug,
                Excerpt = page.Excerpt,
                Day = page.CreateAt.Day,
                Month = page.CreateAt.Month,
                Year = page.CreateAt.Year,
                Minutes = page.CreateAt.Minute,
                Hour = page.CreateAt.Hour,
                Content = page.Content,
                Status = page.Status,
                Category = page.Category
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PostsForm form)
        {
            var page = Database.Session.Load<Post>((Int64)id);
            
            if (page == null) return HttpNotFound();            

            if (!ModelState.IsValid)
                return View(new PostsForm
                {
                    Id = id,
                    Title = page.Title,
                    Slug = page.Slug,
                    Excerpt = page.Excerpt,
                    Day = page.CreateAt.Day,
                    Month = page.CreateAt.Month,
                    Year = page.CreateAt.Year,
                    Minutes = page.CreateAt.Minute,
                    Hour = page.CreateAt.Hour,
                    Content = page.Content,
                    Status = page.Status,
                    Category = null
                });
            
            page.User = Auth.User;

            page.UpdateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0);

            page.Title = form.Title;
            page.Slug = !String.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly();
            page.Excerpt = form.Excerpt;
            page.Content = form.Content;
            page.Type = TypePost;
            page.Status = form.Status;
            page.CommentStatus = "open";
            page.Category = null;
            page.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, page.Slug, page.Id);

            Database.Session.Update(page);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(int id)
        {
            var page = Database.Session.Load<Post>((Int64)id);

            if (page == null) return HttpNotFound();
            
            Database.Session.Update(page);

            return RedirectToAction("Index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var page = Database.Session.Load<Post>((Int64)id);

            if (page == null) return HttpNotFound();

            Database.Session.Delete(page);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }
    }
}