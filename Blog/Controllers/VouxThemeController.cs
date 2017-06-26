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


        private static IEnumerable<PostsShow> AddImageToPost(IEnumerable<Post> posts)
        {
            var listPost = new List<PostsShow>();

            foreach (var post in posts)
            {             
                listPost.Add(new PostsShow(post));
            }

            return listPost;
        }

        private IEnumerable<SidebarTag> CheckPostPublished()
        {
            var term = Database.Session.Query<Term>()
                .Where(t => t.Taxonomy == "tag" && t.Posts.Count > 0);

            var tags = new List<Term>();
            foreach (var item in term)
            {
                var i = item.Posts;
                tags.AddRange(from post in i where post.Type == TypePost && post.Status == "publish" && post.CreateAt <= DateTime.Now select item);
            }
            return  tags.Distinct().Select(t => new { t.Id, t.Name, t.Slug, PostCount = t.Posts.Count })
                .Select(tag => new SidebarTag(tag.Id, tag.Name, tag.Slug, tag.PostCount)).OrderByDescending(t=>t.PostCount).ToList();

        }
        // GET: VouxTheme
        public ActionResult Index(int page = 1)
        {            

            var baseQuery = Database.Session.Query<Post>()
                .Where(t => t.Type == TypePost && t.Status =="publish" && t.CreateAt <= DateTime.Now)
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
            
            ViewBag.RecentPosts = baseQuery
               .Where(t => !ids.Contains(t.Id))
               .Select(t => new PostsShow(t))
               .Take(10)
               .ToList();

            return View(new PostsIndex
            {
                Posts = new PageData<PostsShow>(postList, totalPostCount, page, PostsPerPage)
            });
        }

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

            ViewBag.Tags = ViewBag.Tags = CheckPostPublished();

            return View(new PostsShow(post));
        }
        
        public ActionResult Tag(string slug, int page = 1)
        {            

            if (slug == null) return HttpNotFound();
            var tag = Database.Session.Query<Term>().FirstOrDefault(t => t.Slug == slug);

            if (tag == null) return HttpNotFound();
            
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

            var ids = posts.Select(t => t.Id).ToArray();

            var baseQuery = Database.Session.Query<Post>().Where(t => t.Type == TypePost && t.Status == "publish" && t.CreateAt <= DateTime.Now).OrderByDescending(t => t.CreateAt);
            
            ViewBag.RecentPosts = baseQuery
               .Where(t => !ids.Contains(t.Id))
               .Select(t => new PostsShow(t))
               .Take(10)
               .ToList();

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