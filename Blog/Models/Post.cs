using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Bytecode.CodeDom;
using NHibernate.Linq;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Blog.Models
{
    public enum StatusPost
    {
        Published,
        Draft,
        Deleted
    }
    public class Post
    {
        public virtual long Id { get; set; }        

        public virtual User User { get; set; }

        public virtual string Title { get; set; }

        public  virtual string Slug { get; set; }

        public virtual string Excerpt { get; set; }

        public virtual string Content { get; set; }

        public virtual string Status { get; set; }

        public virtual string CommentStatus { get; set; }

        public virtual string Password { get; set; }

        public virtual long Parent { get; set; }

        public virtual string Guid { get; set; }

        public virtual int MenuOrder { get; set; }

        public  virtual string Type { get; set; }
        
        public virtual int CommentCount { get; set; }


        public virtual DateTime CreateAt { get; set; }
        public virtual DateTime? UpdateAt { get; set; }
        public virtual DateTime? DeleteAt { get; set; }

        public virtual bool IsDeleted => DeleteAt != null;

        public virtual IList<Term> Category { get; set; }        
        public virtual IList<PostMeta> PostMetas { get; set; }        
        public Post()
        {            
            Category = new List<Term>();
            PostMetas = new List<PostMeta>();
        }


        public virtual void CreateKeyValue(long postId, string metaKey, string metaValue = "")
        {
            DeleteMetaKey(postId, metaKey);
            if (!string.IsNullOrEmpty(metaValue))
            {
                var p = new PostMeta
                {
                    PostId = postId,
                    MetaKey = metaKey,
                    MetaValue = metaValue
                };
                Database.Session.Save(p);
            }
        }

        public virtual void DeleteMetaKey(long postId, string metaKey)
        {
            if (postId <= 0) return;            
            var sql = "DELETE FROM postmeta WHERE post_id = ? and meta_key = ?";
            Database.Session.CreateSQLQuery(sql).SetInt64(0, postId).SetString(1, metaKey).ExecuteUpdate();
        }

    }

    public class PostMap : ClassMapping<Post>
    {
        public PostMap()
        {
            Table("posts");

            Id(x=>x.Id, x=>x.Generator(Generators.Identity));

            ManyToOne(x=>x.User, x =>
            {
                x.Column("user_id");
                x.NotNullable(true);
            });


            Property(x=>x.Title, x=>x.NotNullable(true));
            Property(x=>x.Slug, x=>x.NotNullable(true));
            Property(x=>x.Excerpt, x=>x.NotNullable(false));
            Property(x=>x.Content, x=>x.NotNullable(true));
            Property(x => x.Status, x => x.NotNullable(false));
            Property(x => x.Password, x => x.NotNullable(false));
            Property(x => x.Parent, x => x.NotNullable(false));
            Property(x => x.Guid, x => x.NotNullable(false));            
            Property(x => x.Type, x => x.NotNullable(true));            

            Property(x=>x.CreateAt, x =>
            {
                x.Column("created_at");
                x.NotNullable(true);
            });

            Property(x => x.CommentStatus, x =>
            {
                x.Column("comment_status");
                x.NotNullable(false);
            });

            Property(x => x.MenuOrder, x =>
            {
                x.Column("menu_order");
                x.NotNullable(false);
            });

            Property(x => x.MenuOrder, x =>
            {
                x.Column("comment_count");
                x.NotNullable(false);
            });

            Property(x=>x.UpdateAt, x=>x.Column("updated_at"));
            Property(x=>x.DeleteAt, x=>x.Column("deleted_at"));


            Bag(x => x.Category, x =>
            {
                x.Key(y => y.Column("post_id"));
                x.Table("term_posts");                                

            }, x => x.ManyToMany(y =>
                {
                    y.Column("term_id");
                    y.NotFound(NotFoundMode.Ignore);
                }
            ));


            //Bag(x => x.PostMetas, x =>
            //{
            //    x.Key(y => y.Column("post_id"));
            //    x.Table("postmeta");

            //},x=>x.OneToMany());

            Bag(x => x.PostMetas, x =>
            {
                x.Key(y =>
                {
                    y.Column("post_id");                    
                });
                x.Cascade(Cascade.All | Cascade.DeleteOrphans);
                x.Lazy(CollectionLazy.Lazy);
                x.Inverse(true);
            }, x =>
            {
                x.OneToMany();                
            });
        }
        
    }
}