using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class SidebarTag
    {        
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public int PostCount { get; private set; }

        public SidebarTag(long id, string name, string slug, int postCount)
        {
            Id = id;
            Name = name;
            Slug = slug;
            PostCount = postCount;
        }        
    }

    public class LayoutSidebar
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }

        public bool IsAdmin { get; set; }

        public IEnumerable<SidebarTag> Tags { get; set; }
    }
}