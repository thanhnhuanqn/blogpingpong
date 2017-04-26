using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;

namespace Blog.Migrations
{
    [Migration(2)]
    public class _002_PostMeta : Migration
    {        
        public override void Up()
        {
            Create.Table("postmeta")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("post_id").AsInt64().NotNullable()
                .WithColumn("meta_key").AsString(255).Nullable()
                .WithColumn("meta_value").AsCustom("TEXT").Nullable()
                ;

            Create.Table("termmeta")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("term_id").AsInt64().NotNullable()
                .WithColumn("meta_key").AsString(255).Nullable()
                .WithColumn("meta_value").AsCustom("TEXT").Nullable()
                ;

            Create.Table("commentmeta")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("comment_id").AsInt64().NotNullable()
                .WithColumn("meta_key").AsString(255).Nullable()
                .WithColumn("meta_value").AsCustom("TEXT").Nullable()
                ;

            Create.Table("usermeta")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt64().NotNullable()
                .WithColumn("meta_key").AsString(255).Nullable()
                .WithColumn("meta_value").AsCustom("TEXT").Nullable()
                ;            
        }

        public override void Down()
        {
            Delete.Table("usermeta");
            Delete.Table("commentmeta");
            Delete.Table("termmeta");
            Delete.Table("postmeta");
        }
    }
}