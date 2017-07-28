using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using NHibernate.Bytecode.CodeDom;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Blog.Models
{
    public class PostMeta
    {
        public virtual Int32 Id { get; set; }
        public virtual Int64 PostId { get; set; }

        public virtual string MetaKey { get; set; }
        public virtual string MetaValue { get; set; }
    }

    class PostMetaMap : ClassMapping<PostMeta>
    {
        public PostMetaMap()
        {
            Table("postmeta");
            Id(x=>x.Id, x=>x.Generator(Generators.Identity));

            Property(x=>x.PostId, x =>
            {
                x.Column("post_id");
                x.NotNullable(true);
            });
            Property(x => x.MetaKey, x =>
            {
                x.Column("meta_key");
                x.NotNullable(false);
            });
            Property(x => x.MetaValue, x =>
            {
                x.Column("meta_value");
                x.NotNullable(false);
            });

        }
    }
}