using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using FluentMigrator;

namespace Blog.Migrations
{
    [Migration(1)]
    public class _001_TablesBlog : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id")
                .AsInt32()
                .Identity()
                .PrimaryKey()

                .WithColumn("username")
                .AsString(128)

                .WithColumn("password")
                .AsString(128)

                .WithColumn("email")
                .AsString(100)

                .WithColumn("url")
                .AsString(100).Nullable()

                .WithColumn("registered")
                .AsDateTime()

                .WithColumn("activation_key")
                .AsString(60).Nullable()

                .WithColumn("status")
                .AsInt16()
                .WithDefaultValue(0)

                .WithColumn("display_name")
                .AsString(250)
                .Nullable()
            ;
            Create.Table("roles")
                .WithColumn("id")
                .AsInt32()
                .Identity()
                .PrimaryKey()
                .WithColumn("name")
                .AsString(128);

            Create.Table("role_users")
                .WithColumn("user_id").AsInt32().ForeignKey("users", "id").OnDelete(Rule.Cascade)
                .WithColumn("role_id").AsInt32().ForeignKey("roles", "id").OnDelete(Rule.Cascade);

            Create.Table("posts")
                .WithColumn("id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                               
                .WithColumn("user_id")
                .AsInt32()
                .ForeignKey("users", "id")
                .Indexed()


                .WithColumn("created_at")
                .AsDateTime()
                .Indexed()

                .WithColumn("updated_at")
                .AsDateTime()
                .Nullable()

                .WithColumn("deleted_at")
                .AsDateTime()
                .Nullable()

                .WithColumn("title")
                .AsString(255)

                .WithColumn("slug")
                .AsString(200)
                .Nullable()
                .Indexed()

                .WithColumn("excerpt")
                .AsCustom("TEXT")
                .Nullable()

                .WithColumn("content")
                .AsCustom("TEXT")

                .WithColumn("status")
                .AsString(20)
                .WithDefaultValue("publish")

                .WithColumn("comment_status")
                .AsString(20)
                .WithDefaultValue("open")

                .WithColumn("password")
                .AsString(20)
                .Nullable()


                .WithColumn("parent").AsInt64().WithDefaultValue(0)

                .WithColumn("guid")
                .AsString(255)
                .Nullable()

                .WithColumn("menu_order").AsInt32().WithDefaultValue(0)

                .WithColumn("type")
                .AsString(20)
                .WithDefaultValue("post")

                .WithColumn("comment_count")
                .AsInt32()
                .WithDefaultValue(0)
                ;

            Create.Table("terms")
                .WithColumn("id").AsInt64().Identity().PrimaryKey()
                .WithColumn("name").AsString(200).Indexed()
                .WithColumn("slug").AsString(200).Unique()
                .WithColumn("group").AsInt32().WithDefaultValue(0)
                .WithColumn("taxonomy").AsString(32)
                .WithColumn("description").AsCustom("TEXT").Nullable()
                .WithColumn("parent").AsInt64().WithDefaultValue(0)
                .WithColumn("count").AsInt64().WithDefaultValue(0)

                ;

            Create.Table("term_posts")
                .WithColumn("post_id").AsInt64().ForeignKey("posts", "id").OnDelete(Rule.Cascade)
                .WithColumn("term_id").AsInt64().ForeignKey("terms", "id").OnDelete(Rule.Cascade)
                .WithColumn("order").AsInt16().WithDefaultValue(0)
                ;

            Create.Table("options")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(64)
                .WithColumn("value").AsCustom("TEXT")
                .WithColumn("autoload").AsString(20).WithDefaultValue("yes")
                ;


            Create.Table("comments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("post_id").AsInt64().WithDefaultValue(0)
                .WithColumn("author").AsString(100).Nullable()
                .WithColumn("email").AsString(100).Nullable()
                .WithColumn("url").AsString(200).Nullable()
                .WithColumn("ip").AsString(100).Nullable()
                .WithColumn("created").AsDateTime()
                .WithColumn("content").AsCustom("TEXT")
                .WithColumn("karma").AsInt32().WithDefaultValue(0)
                .WithColumn("approved").AsString(20).WithDefaultValue("1")
                .WithColumn("agent").AsString(20).Nullable()
                .WithColumn("type").AsString(20).Nullable()
                .WithColumn("parent").AsInt64().WithDefaultValue(0)
                .WithColumn("user_id").AsInt32().WithDefaultValue(0)

                ;
        }

        public override void Down()
        {
            Delete.Table("role_users");
            Delete.Table("roles");
            Delete.Table("users");
            Delete.Table("term_posts");
            Delete.Table("terms");
            Delete.Table("posts");
            Delete.Table("options");
            Delete.Table("comments");
        }
    }
}