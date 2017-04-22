using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate;
using NHibernate.Linq;
using Paging;

namespace Blog.Areas.admin.Controllers
{
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private string Type = "post";
        private const int PostsPerPage = 50;

        private const int defaultPageSize = 15;
        

        // GET: admin/Posts
        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(Database.Session.Query<Post>().Where(p=>p.Type == Type).OrderByDescending(t=>t.CreateAt).ToPagedList(currentPageIndex, defaultPageSize));
        }

        public ActionResult New()
        {
            return View(new PostsForm
            {             
                Day  = DateTime.Now.Day,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                Hour = DateTime.Now.Hour,
                Minutes = DateTime.Now.Minute,
                Tags = Database.Session.Query<Term>().Where(t=>t.Taxonomy=="tag").ToList(),
                Category = Database.Session.Query<Term>().Where(t=>t.Taxonomy=="cat").ToList()
            });
        }

        private void UpdateOrCreateTag(string tags, Post post)
        {            

            if (!string.IsNullOrEmpty(tags))
            {
                var listTags = tags.Split(',').Where(t => t != string.Empty).ToArray();

                foreach (var tag in listTags)
                {
                    if (!string.IsNullOrWhiteSpace(tag.Trim())) { 
                        var t = Database.Session.Query<Term>().SingleOrDefault(x => x.Slug == tag.UrlFriendly());
                        if (t != null)
                        {
                            post.Category.Add(t);
                        }
                        else
                        {
                            var newTag = new Term
                            {
                                Name = tag.Trim(),
                                Slug = tag.UrlFriendly(),
                                Taxonomy = "tag",
                                Description = "new tag add from post",
                                Count = 1
                            };
                            Database.Session.Save(newTag);
                            Database.Session.Flush();
                            var oldTag = Database.Session.Query<Term>().FirstOrDefault(x => x.Slug == newTag.Slug);
                            if (oldTag != null)
                            {
                                post.Category.Add(oldTag);
                            }
                        }
                    }
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(PostsForm form)
        {
            var categories = Request["categories"];
            var tags = Request["ctags"];

            if (string.IsNullOrEmpty(categories))
            {
                ModelState.AddModelError("categories", "Category is not set");
            }
            if (!ModelState.IsValid)
                return View(new PostsForm
                {
                    Day = DateTime.Now.Day,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Hour = DateTime.Now.Hour,
                    Minutes = DateTime.Now.Minute,
                    Tags = Database.Session.Query<Term>().Where(t => t.Taxonomy == "tag").ToList(),
                    Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList()
                });

            //var selectedTags = ReconsileTags(form.Tags);


            var post = new Post
            {
                CreateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0),
                User = Database.Session.Load<User>(1),
                Title = form.Title,
                Slug = !String.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly(),
                Excerpt = form.Excerpt,
                Content = form.Content,
                Type = "post",
                Status = form.Status,
                CommentStatus = "open"
            };            

            UpdateOrCreateTag(tags, post);

            var listCategories = categories.Split(',').Where(t => t != string.Empty).ToArray();
            
            foreach (var category in listCategories)
            {

                var cat = Database.Session.Load<Term>(Int64.Parse(category));
                
                cat.Count = cat.Posts.Count();
                Database.Session.Update(cat);
                Database.Session.Flush();
                post.Category.Add(cat);
            }


            Database.Session.Save(post);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id)
        {
            //var post = Database.Session.Load<Post>((Int64)id);
            var post = Database.Session.Query<Post>().SingleOrDefault(t => t.Id == id);
                        
            if (post == null) return HttpNotFound();

            ViewBag.Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();

            return View(new PostsForm
            {
                Id = id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Day = post.CreateAt.Day,
                Month = post.CreateAt.Month,
                Year = post.CreateAt.Year,
                Minutes = post.CreateAt.Minute,
                Hour = post.CreateAt.Hour,
                Content = post.Content,
                Status = post.Status,                
                Category =  post.Category
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PostsForm form)
        {
            var post = Database.Session.Load<Post>((Int64)id);
            

            if (post == null) return HttpNotFound();

            ViewBag.Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();

            post.Category = new List<Term>();

            if (!ModelState.IsValid)
                return View(new PostsForm
                {
                    Id = id,
                    Title = post.Title,
                    Slug = post.Slug,
                    Excerpt = post.Excerpt,
                    Day = post.CreateAt.Day,
                    Month = post.CreateAt.Month,
                    Year = post.CreateAt.Year,
                    Minutes = post.CreateAt.Minute,
                    Hour = post.CreateAt.Hour,
                    Content = post.Content,
                    Status = post.Status,                    
                    Category = post.Category
                });

            var categories = Request["categories"];
            var tags = Request["ctags"];

            post.User = Database.Session.Load<User>(1);

            post.UpdateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0);
            
            post.Title = form.Title;
            post.Slug = !String.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly();
            post.Excerpt = form.Excerpt;
            post.Content = form.Content;
            post.Type = "post";
            post.Status = form.Status;
            post.CommentStatus = "open";

            post.Category = null;
            Database.Session.Update(post);

            post.Category = new List<Term>();

            UpdateOrCreateTag(tags, post);
            
            var listCategories = categories.Split(',').Where(t => t != string.Empty).ToArray();

            foreach (var category in listCategories)
            {
                var cat = Database.Session.Load<Term>(Int64.Parse(category));

                if (cat != null)
                {
                    cat.Count = cat.Posts.Count();
                    Database.Session.Update(cat);
                    Database.Session.Flush();
                    post.Category.Add(cat);
                }
            }


            Database.Session.Update(post);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(int id)
        {
            var post = Database.Session.Load<Post>((Int64)id);

            if (post == null) return HttpNotFound();

            post.DeleteAt = DateTime.UtcNow;
            Database.Session.Update(post);

            return RedirectToAction("Index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>((Int64)id);

            if (post == null) return HttpNotFound();

            Database.Session.Delete(post);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Restore(int id)
        {
            var post = Database.Session.Load<Post>((Int64)id);

            if (post == null) return HttpNotFound();

            post.DeleteAt = null;
            Database.Session.Update(post);

            return RedirectToAction("Index");

        }

        private IEnumerable<Term> ReconsileTags(IEnumerable<Term> tags)
        {
            foreach (var tag in tags)
            {
                if (tag.Id > 0 )
                {
                    yield return Database.Session.Load<Term>(tag.Id);
                }

                var existingTag = Database.Session.Query<Term>().FirstOrDefault(t => t.Name == tag.Name);

                if (existingTag != null)
                {
                    yield return existingTag;
                    continue;
                }

                var newTag = new Term
                {
                    Name = tag.Name,
                    Slug = tag.Name.UrlFriendly(),
                    Taxonomy = "tag"
                };

                Database.Session.Save(newTag);
                yield return newTag;
            }
        }
    }
}