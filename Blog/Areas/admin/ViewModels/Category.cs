using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.Areas.admin.ViewModels
{
    public class CategoryIndex
    {
        public IList<Term> Category;
        public CategoryIndex()
        {
            Category = new List<Term>();
        }
    }    

    public class CategoryNew
    {
        public IList<Term> Categories;

        public Int64 Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Slug { get; set; }
        [MaxLength(32)]
        public string Taxonomy { get; set; }
        public string Description { get; set; }

        public Int64 Parent { get; set; }
        public CategoryNew()
        {
            Categories = new List<Term>();
        }
    }
}