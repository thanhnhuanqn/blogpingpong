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
        public PageData<Post> Posts { get; set; }
    }

    public class PostsShow
    {
        public Post Post { get; set; }
    }


    public class PostsTag
    {
        public Term Tag { get; set; }
        public PageData<Post> Posts { get; set; }
    }
}