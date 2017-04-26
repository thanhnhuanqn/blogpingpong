using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;
using NHibernate.Linq;

namespace Blog.Areas.admin.ViewModels
{
    public class Appearence
    {
        public List<Term> Menu { get; set; }
        public List<Post> Pages { get; set; }
        public List<Term> Categories { get; set; }
        public List<Post> CustomLinks { get; set; }
        

        public Appearence()
        {
            Menu = Database.Session.Query<Term>().Where(t => t.Taxonomy == "menu").ToList();
            Pages = Database.Session.Query<Post>().Where(t => t.Type == "page").ToList();
            Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();
            CustomLinks = Database.Session.Query<Post>().Where(t => t.Type == "link").ToList();
            
        }
    }

    public class Nav
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Post> NavItems { get; set; }

        public Appearence DataMenu { get; set; }

        public Nav(Term cat)
        {
            NavItems = Database.Session.Query<Post>().Where(t => t.Type == "nav_menu_item" && t.Parent == Id).OrderBy(t => t.MenuOrder).ToList();
            DataMenu = new Appearence();
            Name = cat.Name;
            Id = cat.Id;
        }
        public Nav()
        {            
            NavItems = Database.Session.Query<Post>().Where(t => t.Type == "nav_menu_item" && t.Parent == Id).OrderBy(t=>t.MenuOrder).ToList();
            DataMenu = new Appearence();
        }
    }
}