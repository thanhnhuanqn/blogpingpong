using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Infrastructure;
using Blog.Services.VouxTheme;

namespace Blog.ViewModels.VouxTheme
{
    public class PostsRecentVoux : VouxThemeService
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string PostImage => GetMetaPost(Id);
    }

    public class PostVoux : PostsRecentVoux
    {
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public int CommentCount { get; set; }

        public List<LabelVoux> Categories => GetCategoriesOfPost(Id);

        public List<LabelVoux> Tags => GetTagsOfPost(Id);
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