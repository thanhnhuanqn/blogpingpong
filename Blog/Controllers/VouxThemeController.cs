using System;
using System.Collections.Generic;
using System.Linq;
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
       
        private static IEnumerable<SidebarTag> CheckPostPublished()
        {
            var term = Database.Session.Query<Term>()                
                .Where(o => o.Taxonomy == "tag" && o.Posts.Any(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now))
                .Distinct();
            
            var tags = new List<SidebarTag>();
            foreach (var item in term)
            {                
                tags.Add(new SidebarTag(item.Id, item.Name, item.Slug, item.Posts.GroupBy(a=>a.Id).Count()));
            }
            return  tags.OrderByDescending(t=>t.PostCount).ToList();

        }

        private static IEnumerable<PostsShow> RecentPosts(IQueryable<Post> source, long[] ids, int take)
        {            
            var recentPosts = source
               .Where(t => !ids.Contains(t.Id))
               .Select(t => new PostsShow(t))
               .Take(take)
               .ToList();

            return recentPosts;
        }
        // GET: VouxTheme
        public ActionResult Index(int page = 1)
        {            

            var baseQuery = Database.Session.Query<Post>()
                .Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now)
                .OrderByDescending(t => t.CreateAt);

            var totalPostCount = baseQuery.Count();
            var postIds = baseQuery.Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Select(t => t.Id).ToArray();

            var posts = baseQuery
                .Where(t => postIds.Contains(t.Id))
                .ToList();

            var ids = posts
                .Select(t => t.Id)
                .ToArray();            

            var postList = posts.Select(t => new PostsShow(t)).ToList();

            ViewBag.Tags = CheckPostPublished();

            ViewBag.RecentPosts = RecentPosts(baseQuery, ids, 10);

            return View(new PostsIndex
            {
                Posts = new PageData<PostsShow>(postList, totalPostCount, page, PostsPerPage)
            });
        }
        /// <summary>
        /// Display Single Post
        /// </summary>
        /// <param name="slug">Slug of Post</param>
        /// <returns>Object PostsShow</returns>
        public ActionResult Show(string slug)
        {
            var slugPost = slug.Trim();

            var post = Database.Session.Query<Post>().FirstOrDefault(t => t.Slug == slugPost);

            if (post == null || post.Status != "publish" || post.CreateAt > DateTime.Now) return HttpNotFound();
            
            var baseQuery = Database.Session.Query<Post>().Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now).OrderByDescending(t => t.CreateAt);
            
            ViewBag.RecentPosts = baseQuery
               .Where(t => t.Id != post.Id)
               .Select(t => new PostsShow(t))
               .Take(10)
               .ToList();

            ViewBag.RecentPosts = RecentPosts(baseQuery, new[] { post.Id } , 20);

            ViewBag.Tags = CheckPostPublished();

            return View(new PostsShow(post));
        }
        /// <summary>
        /// Hiển thị bài viết theo tag
        /// </summary>
        /// <param name="slug">slug của tag</param>
        /// <param name="page">Trang hiện tại</param>
        /// <returns>Object PostsTag</returns>
        public ActionResult Tag(string slug, int page = 1)
        {            
            if (slug == null) return HttpNotFound();

            var tag = Database.Session.Query<Term>()
                .FirstOrDefault(o => o.Slug == slug);

            if (tag == null) return HttpNotFound();

            var totalPostCount = tag.Posts.GroupBy(t => t.Id).Count();

            var postIds = tag.Posts
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now)
                .Select(t => t.Id)
                .ToArray();

            var posts = Database.Session.Query<Post>()
                .OrderByDescending(b => b.CreateAt)
                .Where(t => postIds.Contains(t.Id))                
                .ToList();

            var ids = posts.Select(t => t.Id).ToArray();

            var baseQuery = Database.Session.Query<Post>()
                .Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now)
                .OrderByDescending(t => t.CreateAt);
                       
            ViewBag.RecentPosts = RecentPosts(baseQuery, ids, 10);

            ViewBag.Tags = CheckPostPublished();

            var postList = posts.Select(t => new PostsShow(t)).ToList();
            
            return View(new PostsTag
            {
                Tag = tag,
                Posts = new PageData<PostsShow>(postList, totalPostCount, page, PostsPerPage)
            });
        }
    }
}