using Blog.Areas.admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Blog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var namespaces = new[] {typeof(PostsController).Namespace };


            routes.MapRoute("Home", "", new {controller = "Posts", action = "index"}, namespaces);
        }
    }
}
