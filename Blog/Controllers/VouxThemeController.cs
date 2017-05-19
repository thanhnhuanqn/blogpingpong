using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;

namespace Blog.Controllers
{
    public class VouxThemeController : Controller
    {
        private const int PostsPerPage = 5;

        private const string TypePost = "post";
        // GET: VouxTheme
        public ActionResult Index(int page = 1)
        {            

            var baseQuery = Database.Session.Query<Post>().Where(t => t.Type == TypePost && t.Status =="publish" && t.CreateAt <= DateTime.Now).OrderByDescending(t => t.CreateAt);

            var totalPostCount = baseQuery.Count();
            var postIds = baseQuery.Skip((page - 1) * PostsPerPage).Take(PostsPerPage).Select(t => t.Id).ToArray();

            var posts = baseQuery.Where(t => postIds.Contains(t.Id)).ToList();
            var ids = posts.Select(t => t.Id).ToArray();

            ViewBag.RecenPosts = baseQuery.Where(t=> !ids.Contains(t.Id)).Take(10).ToList();

            return View(new PostsIndex
            {
                Posts = new PageData<Post>(posts, totalPostCount, page, PostsPerPage)
            });
        }



        public ActionResult Show(string slug)
        {
            var slugPost = slug.Trim();

            var post = Database.Session.Query<Post>().FirstOrDefault(t => t.Slug == slugPost);

            if (post == null || post.Status != "publish" || post.CreateAt > DateTime.Now) return HttpNotFound();

            if (!post.Slug.Equals(slugPost, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("Post", new { slug = post.Slug });

            var baseQuery = Database.Session.Query<Post>().Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now).OrderByDescending(t => t.CreateAt);

            ViewBag.RecenPosts = baseQuery.Where(t => t.Id != post.Id).Take(10).ToList();

            return View(new PostsShow
            {
                Post = post
            });
        }


        public ActionResult Tag(string slug, int page = 1)
        {            

            if (slug == null) return HttpNotFound();
            var tag = Database.Session.Query<Term>().FirstOrDefault(t => t.Slug == slug);

            if (tag == null) return HttpNotFound();

            if (!tag.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToActionPermanent("tag", new { slug = tag.Slug });
            }

            var totalPostCount = tag.Posts.Count();

            var postIds = tag.Posts
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Where(t => t.Type == "post" && t.Status=="publish" && t.CreateAt <= DateTime.Now)
                .Select(t => t.Id)
                .ToArray();

            var posts = Database.Session.Query<Post>()
                .OrderByDescending(b => b.CreateAt)
                .Where(t => postIds.Contains(t.Id))                
                .ToList();
            var baseQuery = Database.Session.Query<Post>().Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now).OrderByDescending(t => t.CreateAt);

            var ids = posts.Select(t => t.Id).ToArray();

            ViewBag.RecenPosts = baseQuery.Where(t => !ids.Contains(t.Id)).Take(10).ToList();

            return View(new PostsTag
            {
                Tag = tag,
                Posts = new PageData<Post>(posts, totalPostCount, page, PostsPerPage)
            });
        }
    }
}