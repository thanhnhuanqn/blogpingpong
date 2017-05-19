using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;
using Blog.Module.Paging;

namespace Blog.Areas.admin.ViewModels
{

    public class TermCheckBox
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class PageIndex
    {
        public IPagedList<Post> Posts { get; set; }
    }
    public class PostsIndex
    {
        public PageData<Post> Posts { get; set; }
    }
    public class PostsForm
    {        
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }

        public virtual long Id { get; set; }

        public virtual User User { get; set; }

        [Required(ErrorMessage = "Title cannot empty"), MaxLength(255)]
        public virtual string Title { get; set; }

        public virtual string Slug { get; set; }


        [DataType(DataType.MultilineText), AllowHtml]
        public virtual string Excerpt { get; set; }


        [Required(ErrorMessage = "Content cannot empty"), DataType(DataType.MultilineText), AllowHtml]
        public virtual string Content { get; set; }

        public virtual string Status { get; set; }

        public virtual string CommentStatus { get; set; }

        public virtual string Password { get; set; }

        public virtual Int64 Parent { get; set; }

        public virtual string Guid { get; set; }

        public virtual int MenuOrder { get; set; }

        public virtual string Type { get; set; }

        public virtual int CommentCount { get; set; }

        public virtual string Keyword { get; set; }

        public virtual DateTime CreateAt { get; set; }
        public virtual DateTime? UpdateAt { get; set; }
        
        public virtual IList<Term> Category { get; set; }
        public virtual IList<Term> Tags { get; set; }

        public virtual IList<Post> Images { get; set; }

        public virtual string Sticky { get; set; }

        public PostsForm()
        {
            Tags = Database.Session.Query<Term>().Where(t => t.Taxonomy == "tag").ToList();
            Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();
            Images = Database.Session.Query<Post>().Where(t => t.Type == "image").ToList();
        }



    }
}