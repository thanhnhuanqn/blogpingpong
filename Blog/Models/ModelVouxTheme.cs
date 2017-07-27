using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Blog.Infrastructure;
using Dapper;
using MySql.Data.MySqlClient;

namespace Blog.Models
{
    public class PostVoux
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public int CommentCount { get; set; }

        private readonly IDbConnection _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);

        public List<LabelVoux> Categories
        {
            get
            {
                var query =
                    "select t.id, t.slug, t.name " +
                    "from terms t, posts p, term_posts tp " +
                    "where t.id = tp.term_id and tp.post_id = p.id and t.taxonomy = 'cat' and p.id =" + Id;
                return (List<LabelVoux>)_db.Query<LabelVoux>(query);
            }
        }

        public List<LabelVoux> Tags
        {
            get
            {
                var query =
                    "select t.id, t.slug, t.name " +
                    "from terms t, posts p, term_posts tp " +
                    "where t.id = tp.term_id and tp.post_id = p.id and t.taxonomy = 'tag' and p.id =" + Id;
                return (List<LabelVoux>)_db.Query<LabelVoux>(query);
            }
        }
    }



    public class PostsRecentVoux
    {
        readonly IDbConnection _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);

        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }        
        public string PostImage
        {
            get
            {
                var queryCountPost = "SELECt guid " +
                                     "FROM posts " +
                                     "WHERE posts.id in (" +
                                     "SELECT meta_value " +
                                     "FROM postmeta m, posts p " +
                                     "WHERE p.id = m.post_id " +
                                     "AND meta_key = 'thumbnail_id' " +
                                     "AND post_id = " + Id + ")";

                return _db.Query<string>(queryCountPost).FirstOrDefault();
            }            
        }

    }

    public class LabelVoux
    {
        
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int PostCount { get; set; }        
    }

    public class PostsVouxIndex
    {
        public PageData<PostsVouxShow> Posts { get; set; }
    }

    public class PostsVouxShow
    {
        public string PostImage;
        public PostVoux Post { get; set; }

        public PostsVouxShow(PostVoux post)
        {
            IDbConnection db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);

            Post = post;

            var queryCountPost = "SELECt guid " +
                                 "FROM posts " +
                                 "WHERE posts.id in (" +
                                                    "SELECT meta_value " +
                                                    "FROM postmeta m, posts p " +
                                                    "WHERE p.id = m.post_id " +
                                                            "AND meta_key = 'thumbnail_id' " +
                                                            "AND post_id = "+ post.Id + ")";

            PostImage = db.Query<string>(queryCountPost).Single();
                                    

        }
    }

    public class PostsVouxTag
    {
        public LabelVoux Tag { get; set; }
        public PageData<PostsVouxShow> Posts { get; set; }
    }
}