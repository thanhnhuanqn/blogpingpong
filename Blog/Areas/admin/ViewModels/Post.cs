using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.Areas.admin.ViewModels
{

    public class CategoryCheckBox
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class PostsIndex
    {
        public IList<Post> Posts { get; set; }
    }
}