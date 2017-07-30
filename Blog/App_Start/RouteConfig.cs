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
            //routes.MapMvcAttributeRoutes();

            var adminNamespaces = new[] {typeof(PostsController).Namespace };

            var userNamespaces = new[] {typeof(Blog.Controllers.VouxThemeController).Namespace };

            //code moi
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Posts", action = "Index", id = UrlParameter.Optional },
            //    namespaces: adminNamespaces
            //).DataTokens.Add("Area", "Admin");


            routes.MapRoute(
               name: "Image",
               url: "Image/{imageFile}",
               defaults: new { controller = "Image", action = "Index", id = UrlParameter.Optional }
           );

            //routes.MapRoute("PostForRealThisTime", "{slug}", new { controller = "VouxTheme", action = "Show" }, userNamespaces);
            routes.MapRoute("article", "{slug}", new { controller = "VouxTheme", action = "Show" }, userNamespaces);

            //routes.MapRoute("TagForRealThisTime", "tag/{slug}", new { controller = "VouxTheme", action = "Tag" }, userNamespaces);
            routes.MapRoute("Tag", "tag/{slug}", new { controller = "VouxTheme", action = "Tag" }, userNamespaces);

            //routes.MapRoute("CategoryForRealThisTime", "category/{slug}", new { controller = "VouxTheme", action = "Tag" }, userNamespaces);
            routes.MapRoute("Category", "category/{slug}", new { controller = "VouxTheme", action = "Show" }, userNamespaces);

            routes.MapRoute("Login", "login", new { controller = "Auth", action = "Login", Areas = "admin" }, adminNamespaces);

            routes.MapRoute("Home", "", new {controller = "VouxTheme", action = "index"}, userNamespaces);

            routes.MapRoute("Error500", "errors/500", new {controller = "Errors", action = "Error"}, userNamespaces);
            routes.MapRoute("Error404", "errors/404", new { controller = "Errors", action = "NotFound" }, userNamespaces);
        }
    }
}
