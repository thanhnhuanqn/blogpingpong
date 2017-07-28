using System.Linq;
using System.Web.Mvc;
using Blog.Infrastructure;
using Blog.Services.VouxTheme;
using Blog.ViewModels.VouxTheme;

namespace Blog.Controllers
{
    public class VouxThemeController : Controller
    {
        private const int PostsPerPage = 5;

        private const string TypePost = "post";        

        private readonly IVouxThemeService _data;
        
        public VouxThemeController(IVouxThemeService data)
        {
            _data = data;
        }
        public ActionResult Index(int page = 1)
        {
            
            var totalPostCount = _data.CountPostTypeAndStatus(TypePost, "publish");
            
            var posts = _data.GetAllPostsPublish(page, PostsPerPage);

            var ids = posts
                .Select(t => t.Id)
                .ToArray();

            ViewBag.RecentPosts = _data.RecentVouexPosts(ids, 10);

            ViewBag.Tags = _data.GetTags();            
                        
            return View(new PostsVouxIndex
            {
                Posts = new PageData<PostVoux>(posts, totalPostCount, page, PostsPerPage)
            });
        }
        /// <summary>
        /// Display Single Post
        /// </summary>
        /// <param name="slug">Slug of Post</param>
        /// <returns>Object PostsShow</returns>
        public ActionResult Show(string slug)
        {            
            var post = _data.GetPost(slug);

            if (post == null) return HttpNotFound();            

            ViewBag.RecentPosts = _data.RecentVouexPosts(new []{post.Id}, 10);

            ViewBag.Tags = _data.GetTags();

            return View(post);
        }
        /// <summary>
        /// Hiển thị bài viết theo tag
        /// </summary>
        /// <param name="slug">slug của tag</param>
        /// <param name="page">Trang hiện tại</param>
        /// <returns>Object PostsTag</returns>
        public ActionResult Tag(string slug, int page = 1)
        {            
            var tag = _data.GetCategory(slug);

            if (tag == null) return HttpNotFound();
            
            var totalPostCount = _data.CountPostPublishOfCategory(tag.Id);
            
            var posts = _data.GetPostsOfCategory(tag.Id, page, PostsPerPage);

            var ids = posts.Select(t => t.Id).ToArray();

            ViewBag.RecentPosts = _data.RecentVouexPosts(ids, 10);

            ViewBag.Tags = _data.GetTags();
            
            return View(new PostsVouxTag
            {
                Tag = tag,
                Posts = new PageData<PostVoux>(posts, totalPostCount, page, PostsPerPage)
            });
        }
    }
}