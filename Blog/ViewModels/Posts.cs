using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Infrastructure;
using Blog.Models;

namespace Blog.ViewModels
{
    public class PostsIndex
    {
        public PageData<PostsShow> Posts { get; set; }
    }

    public class PostsShow
    {
        public string PostImage;
        public Post Post { get; set; }
                
        public PostsShow(Post post)
        {
            Post = post;
            var meta = Post.PostMetas.FirstOrDefault(t => t.PostId == Post.Id && t.MetaKey == "thumbnail_id");

            if (meta == null) return;

            var image = Database.Session.Load<Post>(long.Parse(meta.MetaValue));
            if (image != null)
            {
                PostImage = image.Guid;
            }
        }
    }


    public class PostsTag
    {
        public Term Tag { get; set; }
        public PageData<PostsShow> Posts { get; set; }
    }

    public class PostImage
    {
        public long Id { get; set; }
        public string Guid { get; set;}

        public PostImage(long id, string guid)
        {
            Id = id;
            Guid = guid;
        }
    }
}