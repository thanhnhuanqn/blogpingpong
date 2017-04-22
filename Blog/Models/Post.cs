using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Bytecode.CodeDom;
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
        public virtual Int64 Id { get; set; }        

        public virtual User User { get; set; }

        public virtual string Title { get; set; }

        public  virtual string Slug { get; set; }

        public virtual string Excerpt { get; set; }

        public virtual string Content { get; set; }

        public virtual string Status { get; set; }

        public virtual string CommentStatus { get; set; }

        public virtual string Password { get; set; }

        public virtual Int64 Parent { get; set; }

        public virtual string Guid { get; set; }

        public virtual int MenuOrder { get; set; }

        public  virtual string Type { get; set; }
        
        public virtual int CommentCount { get; set; }


        public virtual DateTime CreateAt { get; set; }
        public virtual DateTime? UpdateAt { get; set; }
        public virtual DateTime? DeleteAt { get; set; }

        public virtual bool IsDeleted { get { return DeleteAt != null; } }

        public virtual IList<Term> Category { get; set; }        
        public Post()
        {            
            Category = new List<Term>();
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

            }, x => x.ManyToMany(y => y.Column("term_id")));



        }
        
    }
}