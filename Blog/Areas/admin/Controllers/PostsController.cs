using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Areas.admin.Controllers
{
    public class PostsController : Controller
    {
        // GET: admin/Posts
        public ActionResult Index()
        {
            return View();
        }
    }
}