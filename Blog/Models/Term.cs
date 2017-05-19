using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Blog.Models
{
    public class Term
    {
        public virtual Int64 Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }
        //public virtual int Group { get; set; }
        public virtual string Taxonomy { get; set; }
        public virtual string Description { get; set; }
        public virtual Int64 Parent { get; set; }
        public virtual Int64 Count { get; set; }

        public virtual IList<Post> Posts { get; set; }

        public Term()
        {
            Posts = new List<Post>();
        }
    }

    public class TermMap : ClassMapping<Term>
    {
        public TermMap()
        {
            Table("Terms");
            Id(x=>x.Id, x=>x.Generator(Generators.Identity));

            Property(x=>x.Name, x=>x.NotNullable(true));
            Property(x=>x.Slug, x=>x.NotNullable(true));
            //Property(x=>x.Group, x=>x.NotNullable(false));
            Property(x=>x.Taxonomy, x=>x.NotNullable(false));
            Property(x=>x.Description, x=>x.NotNullable(false));
            Property(x=>x.Parent, x=>x.NotNullable(false));
            Property(x=>x.Count, x=>x.NotNullable(false));

            Bag(x => x.Posts, x =>
            {
                x.Key(y => y.Column("term_id"));
                x.Table("term_posts");
                //x.Cascade(Cascade.DeleteOrphans);

            }, x => x.ManyToMany(y =>
            {
                y.Column("post_id");
                y.NotFound(NotFoundMode.Ignore);                
            }));
        }
    }
}