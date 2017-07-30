using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Blog.ViewModels.VouxTheme;
using Dapper;
using MySql.Data.MySqlClient;

namespace Blog.Services.VouxTheme
{
    public interface IVouxThemeService
    {
        int CountPostTypeAndStatus(string type, string status);
        List<PostVoux> GetAllPostsPublish(int page, int perPage);
        PostVoux GetPost(string slugPost);
        LabelVoux GetCategory(string slug);
        int CountPostPublishOfCategory(long id);
        List<PostVoux> GetPostsOfCategory(long idCategory, int page, int perPage);
        List<PostsRecentVoux> RecentVouexPosts(IEnumerable<long> ids, int take);
        IEnumerable<LabelVoux> GetTags();
        string GetMetaPost(long postId);
        List<LabelVoux> GetCategoriesOfPost(long idPost);
        List<LabelVoux> GetTagsOfPost(long idPost);
    }

    public class VouxThemeService : IVouxThemeService
    {
        protected static IDbConnection Db => new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);

        /// <summary>
        /// Đếm số bài viết có kiểu dữ liệu là @type và trạng thái của bài viết @status
        /// </summary>
        /// <param name="type">"post"</param>
        /// <param name="status">"publish"</param>
        /// <returns>@int</returns>
        public int CountPostTypeAndStatus(string type, string status)
        {
            var queryCountPost =
               "SELECT COUNT(*) " +
               "FROM posts " +
               "WHERE type ='" + type + "' AND status = '" + status + "'";

            return Db.Query<int>(queryCountPost).Single();
        }

        /// <summary>
        /// Lấy tất cả bài viết được publish, và phân trang chúng
        /// </summary>
        /// <param name="page">Trang hiện tại đang đứng</param>
        /// <param name="perPage">Số bài trong một trang</param>
        /// <returns>@List(PostVoux)</returns>
        public List<PostVoux> GetAllPostsPublish(int page, int perPage)
        {
            var query =
                "SELECT users.display_name as username, posts.id, title, posts.slug, excerpt, created_at as created, updated_at as updated, type, posts.status, user_id as userid " +
                "FROM posts " +
                "LEFT JOIN users ON (users.id = posts.user_id) " +
                "WHERE type ='post' AND posts.status = 'publish' " +
                "ORDER BY id DESC " +
                "LIMIT " + perPage + " OFFSET " + (page - 1) * perPage;
            var result = (List<PostVoux>) Db.Query<PostVoux>(query);
            GetCategoriesAndTagForPosts(result);
            return result;
        }
        /// <summary>
        /// Lấy danh mục và các tag cho 1 danh sách bài viết
        /// </summary>
        /// <param name="posts">Một mảng danh sách bài viết cần lấy Category và Tag</param>
        private void GetCategoriesAndTagForPosts(List<PostVoux> posts)
        {
            if (posts == null) return ;

            foreach (var t in posts)
            {
                GetCategoriesAndTagForPost(t);
            }
        }

        /// <summary>
        /// Lấy danh mục, các tag và hình ảnh cho một bài viết
        /// </summary>
        /// <param name="post">@object Bài viết cần lấy Categories, tags và Hình đại diện của bài viết</param>
        private void GetCategoriesAndTagForPost(PostVoux post)
        {
            if (post == null) return ;

            post.Categories = GetCategoriesOfPost(post.Id);
            post.Tags = GetTagsOfPost(post.Id);
            post.PostImage = GetMetaPost(post.Id);
        }
        /// <summary>
        /// Lấy các hình ảnh đại diện cho một Mảng bài viết
        /// </summary>
        /// <param name="posts">Array - Mảng các bài viết cần lấy hình</param>
        private void GetMetaForPosts(List<PostsRecentVoux> posts)
        {
            if (posts == null) return;
            foreach (var post in posts)
            {
                post.PostImage = GetMetaPost(post.Id);
            }
        }   
        
        /// <summary>
        /// Lấy một bài viết theo slug của nó
        /// </summary>
        /// <param name="slugPost">@slug</param>
        /// <returns>@PostVoux</returns>
        public PostVoux GetPost(string slugPost)
        {
            var query = "SELECT id, title, slug, excerpt, content, created_at as created, updated_at as updated " +
                        "FROM posts p " +
                        "WHERE status='publish' " +
                          "AND type='post' " +
                          "AND slug = '" + slugPost.Trim() + "'";
            var result = Db.Query<PostVoux>(query).SingleOrDefault();

            if (result == null) return null;

            GetCategoriesAndTagForPost(result);

            return result;
        }
        /// <summary>
        /// Lấy một Category đầy đủ thông tin dựa vào slug của nó
        /// </summary>
        /// <param name="slug">@slug của category</param>
        /// <returns>@LabelVoux</returns>
        public LabelVoux GetCategory(string slug)
        {
            var query = "SELECT * FROM terms t WHERE t.slug = '" + slug.Trim() + "'";

            return Db.Query<LabelVoux>(query).SingleOrDefault();
        }

        public int CountPostPublishOfCategory(long id)
        {

            var queryCount =
                "SELECT count(*) " +
                "FROM term_posts tp, terms t, posts p " +
                "LEFT JOIN users ON(users.id = p.user_id) " +
                "WHERE tp.post_id = p.id " +
                    "AND t.id = tp.term_id " +
                    "AND p.status = 'publish' " +
                    "AND p.type = 'post' and t.id = " + id;

            return Db.Query<int>(queryCount).SingleOrDefault();
        }

        public List<PostVoux> GetPostsOfCategory(long idCategory, int page, int perPage)
        {
            var queryMain =
              "SELECT users.display_name as username, p.id, title, p.slug, excerpt, content, created_at as created, " +
                      "updated_at as updated, type, p.status, user_id as userid " +
              "FROM term_posts tp, terms t, posts p " +
              "LEFT JOIN users ON(users.id = p.user_id) " +
              "WHERE tp.post_id = p.id " +
                  "AND t.id = tp.term_id " +
                  "AND p.status = 'publish' " +
                  "AND p.type = 'post' and t.id = " + idCategory +
              " ORDER BY id DESC " +
              "LIMIT " + perPage + " OFFSET " + (page - 1) * perPage;

            var result = (List<PostVoux>)Db.Query<PostVoux>(queryMain);
            GetCategoriesAndTagForPosts(result);
            return result;
        }

        public List<PostsRecentVoux> RecentVouexPosts(IEnumerable<long> ids, int take)
        {
            var query =
                "SELECT id, title, slug, created_at as created, updated_at as updated " +
                "FROM posts " +
                "WHERE type ='post' AND status = 'publish' AND id NOT IN (" + string.Join(",", ids) + ") " +
                "ORDER BY id DESC " +
                "LIMIT " + take;

            var results = (List<PostsRecentVoux>)Db.Query<PostsRecentVoux>(query);

            GetMetaForPosts(results);

            return results;
        }

        public IEnumerable<LabelVoux> GetTags()
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

            return Db.Query<LabelVoux>(query);
        }

        public string GetMetaPost(long postId)
        {
            var query = "SELECT guid " +
                                    "FROM posts " +
                                    "WHERE posts.id IN (" +
                                                       "SELECT meta_value " +
                                                       "FROM postmeta m, posts p " +
                                                       "WHERE p.id = m.post_id " +
                                                           "AND meta_key = 'thumbnail_id' " +
                                                           "AND post_id = " + postId + ")";

            return Db.Query<string>(query).FirstOrDefault();
        }
        public string GetMetaPost(long postId, string metaKey)
        {
            var query = "SELECT guid " +
                                    "FROM posts " +
                                    "WHERE posts.id IN (" +
                                                       "SELECT meta_value " +
                                                       "FROM postmeta m, posts p " +
                                                       "WHERE p.id = m.post_id " +
                                                           "AND meta_key = '"+ metaKey + "' " +
                                                           "AND post_id = " + postId + ")";

            return Db.Query<string>(query).FirstOrDefault();
        }

        public List<LabelVoux> GetCategoriesOfPost(long idPost)
        {
            var query =
                    "SELECT t.id, t.slug, t.name " +
                    "FROM terms t, posts p, term_posts tp " +
                    "WHERE t.id = tp.term_id " +
                        "AND tp.post_id = p.id " +
                        "AND t.taxonomy = 'cat' " +
                        "AND p.id =" + idPost;
            return (List<LabelVoux>)Db.Query<LabelVoux>(query);
        }

        public List<LabelVoux> GetTagsOfPost(long idPost)
        {
            var query =
                    "SELECT t.id, t.slug, t.name " +
                    "FROM terms t, posts p, term_posts tp " +
                    "WHERE t.id = tp.term_id " +
                        "AND tp.post_id = p.id " +
                        "AND t.taxonomy = 'tag' " +
                        "AND p.id =" + idPost;
            return (List<LabelVoux>)Db.Query<LabelVoux>(query);
        }
    }
}