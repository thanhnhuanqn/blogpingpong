using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Blog.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using Dapper;
using MySql.Data.MySqlClient;
using NHibernate.Linq;

namespace Blog.Controllers
{
    public class VouxThemeController : Controller
    {
        private const int PostsPerPage = 5;

        private const string TypePost = "post";

        private readonly IDbConnection _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);        

        private IEnumerable<PostsRecentVoux> RecentVouexPosts(IEnumerable<long> ids, int take)
        {            
            var query =
                "SELECT id, title, slug, created_at as created, updated_at as updated " +
                "FROM posts " +
                "WHERE type ='post' and status = 'publish' AND id NOT IN (" + string.Join(",", ids) + ") " +
                "ORDER BY id DESC " +
                "LIMIT " + take;

            return _db.Query<PostsRecentVoux>(query);
        }

        private IEnumerable<LabelVoux> GetTags()
        {
            var query =
                "SELECT t.id, t.name, t.slug, count(*) AS PostCount " +
                "FROM term_posts tp, posts p, terms t " +
                "WHERE tp.post_id = p.id " +
                    "AND t.id = tp.term_id " +
                    "AND t.taxonomy = 'tag' " +
                    "AND p.type = 'post' " +
                    "AND p.status = 'publish' " +
                "GROUP BY t.id, t.name " +
                "ORDER BY PostCount DESC" 
                ;

            return _db.Query<LabelVoux>(query);
        }
        public ActionResult Index(int page = 1)
        {

            var queryCountPost =
                "SELECT count(*) " +
                "FROM posts " +
                "WHERE type ='post' and status = 'publish'";//and created_at <= " + DateTime.Now;

            var totalPostCount = _db.Query<int>(queryCountPost).Single();

            var query =
                "SELECT users.display_name as username, posts.id, title, posts.slug, excerpt, created_at as created, updated_at as updated, type, posts.status, user_id as userid " +
                "FROM posts " +
                "LEFT JOIN users ON (users.id = posts.user_id) " +
                "WHERE type ='post' and posts.status = 'publish' " +
                "ORDER BY id DESC " +
                "LIMIT " + PostsPerPage + " OFFSET " + (page -1) * PostsPerPage;
                        
            var posts = (List<PostVoux>)_db.Query<PostVoux>(query);

            var ids = posts
                .Select(t => t.Id)
                .ToArray();

            ViewBag.RecentPosts = RecentVouexPosts(ids, 10);

            ViewBag.Tags = GetTags();

            var postList = posts.Select(t => new PostsVouxShow(t)).ToList();
                        
            return View(new PostsVouxIndex
            {
                Posts = new PageData<PostsVouxShow>(postList, totalPostCount, page, PostsPerPage)
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

            var query = "SELECT id, title, slug, excerpt, content, created_at as created, updated_at as updated " +
                        "from posts p " +
                        "WHERE status='publish' and type='post' and slug = '" + slugPost + "'";

            var post = _db.Query<PostVoux>(query).SingleOrDefault();

            if (post == null) return HttpNotFound();            

            ViewBag.RecentPosts = RecentVouexPosts(new []{post.Id}, 10);

            ViewBag.Tags = GetTags();

            return View(new PostsVouxShow(post));
        }
        /// <summary>
        /// Hiển thị bài viết theo tag
        /// </summary>
        /// <param name="slug">slug của tag</param>
        /// <param name="page">Trang hiện tại</param>
        /// <returns>Object PostsTag</returns>
        public ActionResult Tag(string slug, int page = 1)
        {
            var slugTag = slug.Trim();

            var query = "select * from terms t where t.slug = '" + slugTag + "'";

            var tag = _db.Query<LabelVoux>(query).SingleOrDefault();

            if (tag == null) return HttpNotFound();

            var queryCount =
                "SELECT count(*) " +
                "FROM term_posts tp, terms t, posts p " +
                "LEFT JOIN users ON(users.id = p.user_id) " +
                "WHERE tp.post_id = p.id " +
                    "AND t.id = tp.term_id " +
                    "AND p.status = 'publish' " +
                    "AND p.type = 'post' and t.id = " + tag.Id;

            var totalPostCount = _db.Query<int>(queryCount).SingleOrDefault();

            var queryMain =
               "SELECT users.display_name as username, p.id, title, p.slug, excerpt, content, created_at as created, " +
                       "updated_at as updated, type, p.status, user_id as userid " +
               "FROM term_posts tp, terms t, posts p " +
               "LEFT JOIN users ON(users.id = p.user_id) " +
               "WHERE tp.post_id = p.id " +
                   "AND t.id = tp.term_id " +
                   "AND p.status = 'publish' " +
                   "AND p.type = 'post' and t.id = " + tag.Id +
               " ORDER BY id DESC " +
               "LIMIT " + PostsPerPage + " OFFSET " + (page - 1) * PostsPerPage;

            var posts = (List<PostVoux>)_db.Query<PostVoux>(queryMain);

            var ids = posts
                .Select(t => t.Id)
                .ToArray();
            ViewBag.RecentPosts = RecentVouexPosts(ids, 10);

            ViewBag.Tags = GetTags();

            var postList = posts.Select(t => new PostsVouxShow(t)).ToList();
            
            return View(new PostsVouxTag
            {
                Tag = tag,
                Posts = new PageData<PostsVouxShow>(postList, totalPostCount, page, PostsPerPage)
            });
        }
    }
}