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
    public class SingletonDb
    {
        private static SingletonDb _dbInstance;
        private readonly IDbConnection _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);

        private SingletonDb()
        {
        }

        public static SingletonDb GetDbInstance()
        {
            return _dbInstance ?? (_dbInstance = new SingletonDb());
        }

        public IDbConnection GetDbConnection()
        {            
            return _db;
        }
    }
    public class DatabaseMySql
    {
        protected static IDbConnection Db => new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServerConnString"].ConnectionString);
    }

    public class PostVoux : PostsRecentVoux
    {        
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public int CommentCount { get; set; }

        public List<LabelVoux> Categories
        {
            get
            {
                var query =
                    "SELECT t.id, t.slug, t.name " +
                    "FROM terms t, posts p, term_posts tp " +
                    "WHERE t.id = tp.term_id " +
                        "AND tp.post_id = p.id " +
                        "AND t.taxonomy = 'cat' " +
                        "AND p.id =" + Id;
                return (List<LabelVoux>)Db.Query<LabelVoux>(query);
            }
        }

        public List<LabelVoux> Tags
        {
            get
            {
                var query =
                    "SELECT t.id, t.slug, t.name " +
                    "FROM terms t, posts p, term_posts tp " +
                    "WHERE t.id = tp.term_id " +
                        "AND tp.post_id = p.id " +
                        "AND t.taxonomy = 'tag' " +
                        "AND p.id =" + Id;
                return (List<LabelVoux>)Db.Query<LabelVoux>(query);
            }
        }
    }



    public class PostsRecentVoux : DatabaseMySql
    {

        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }        
        public string PostImage
        {
            get
            {
                var queryCountPost = "SELECT guid " +
                                     "FROM posts " +
                                     "WHERE posts.id IN (" +
                                                        "SELECT meta_value " +
                                                        "FROM postmeta m, posts p " +
                                                        "WHERE p.id = m.post_id " +
                                                            "AND meta_key = 'thumbnail_id' " +
                                                            "AND post_id = " + Id + ")";

                return Db.Query<string>(queryCountPost).FirstOrDefault();
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
        public PageData<PostVoux> Posts { get; set; }
    }
    public class PostsVouxTag
    {
        public LabelVoux Tag { get; set; }
        public PageData<PostVoux> Posts { get; set; }
    }
}