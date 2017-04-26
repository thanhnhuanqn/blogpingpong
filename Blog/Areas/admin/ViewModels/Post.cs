using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;
using Paging;

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
    public class PostsForm
    {        
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }

        public virtual Int64 Id { get; set; }

        public virtual User User { get; set; }

        [Required, MaxLength(255)]
        public virtual string Title { get; set; }

        public virtual string Slug { get; set; }


        [DataType(DataType.MultilineText), AllowHtml]
        public virtual string Excerpt { get; set; }


        [Required, DataType(DataType.MultilineText), AllowHtml]
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

        public virtual string Sticky { get; set; }

        public PostsForm()
        {
            Tags = new List<Term>();
            Category = new List<Term>();            
        }



    }
}