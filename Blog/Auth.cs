using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using NHibernate.Linq;

namespace Blog
{
    //public class AreaAuthorizeAttribute : AuthorizeAttribute
    //{
    //    private readonly string _area;

    //    public AreaAuthorizeAttribute(string area)
    //    {
    //        this._area = area;
    //    }

    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        var loginUrl = "";

    //        switch (_area)
    //        {
    //            case "Admin":
    //                loginUrl = "~/Admin/Login";
    //                break;
    //            case "Members":
    //                loginUrl = "~/Members/Login";
    //                break;
    //        }

    //        filterContext.Result = new RedirectResult(loginUrl + "?returnUrl=" + filterContext.HttpContext.Request.Url.PathAndQuery);
    //    }
    //}

    public class Auth
    {
        private const string UserKey = "SimpleBlog.Auth.UserKey";

        public static User User
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated) return null;

                var user = HttpContext.Current.Items[UserKey] as User;

                if (user == null)
                {
                    user = Database.Session.Query<User>().FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);

                    if (user == null) return null;

                    HttpContext.Current.Items[UserKey] = user;
                }

                return user;
            }
        }
    }
}