using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using NHibernate.Linq;
using Blog.Module.Paging;

namespace Blog.Areas.admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("Posts")]
    public class PostsController : Controller
    {
        private const string PostType = "post";

        private const int DefaultPageSize = 15;

        public static bool CheckSlugUnique(long id, string slug)
        {
            var post = Database.Session.Query<Post>().Where(p => p.Slug == slug);

            if (id <= 0) return post.Any();

            post = Database.Session.Query<Post>().Where(p => p.Id != id && p.Slug == slug);

            return post.Any();

        }

        // GET: admin/Posts
        public ActionResult Index(int? page, string status, string category, string tag, string user, string search)
        {
            var currentPageIndex = page - 1 ?? 0;

            var posts = Database.Session.Query<Post>().Where(t => t.Type == "post");

            if (!string.IsNullOrEmpty(status))
                posts = Database.Session.Query<Post>().Where(t => t.Type == "post" && t.Status == status.Trim());

            if (category != null)
            {
                var cat = Database.Session.Load<Term>(long.Parse(category));
                if (cat != null)
                {
                    posts = Database.Session.Query<Post>().Where(t => t.Type == "post" && t.Category.Contains(cat));
                }
            }

            if (tag != null)
            {
                var cat = Database.Session.Load<Term>(long.Parse(tag));
                if (cat != null)
                {
                    posts = Database.Session.Query<Post>().Where(t => t.Type == "post" && t.Category.Contains(cat));
                }
            }

            if (!string.IsNullOrEmpty(user))
            {
                var userPost = Database.Session.Load<User>(int.Parse(user));
                if (userPost != null)
                {
                    posts = Database.Session.Query<Post>().Where(t => t.Type == "post" && t.User.Id == userPost.Id);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                posts = Database.Session.Query<Post>().Where(t => t.Type == "post" && t.Title.Contains(search));
            }


            return View(posts.OrderByDescending(t => t.CreateAt).ToPagedList(currentPageIndex, DefaultPageSize));
        }

        public ActionResult New()
        {
            return View(new PostsForm
            {
                Day = DateTime.Now.Day,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                Hour = DateTime.Now.Hour,
                Minutes = DateTime.Now.Minute
            });
        }

        public static void AddCategoriesTagsPost(Post post, string listTag, string categories)
        {

            if (post == null) return;

            var tags = new List<Term>();

            if (!string.IsNullOrEmpty(listTag))
            {
                var listTags = listTag.Split(',').Where(t => t != string.Empty).Distinct().ToArray();

                foreach (var tag in listTags)
                {
                    if (string.IsNullOrWhiteSpace(tag.Trim())) continue;

                    var t = Database.Session.Query<Term>().SingleOrDefault(x => x.Slug == tag.UrlFriendly());
                    if (t != null)
                    {
                        tags.Add(t);
                    }
                    else
                    {
                        var newTag = new Term
                        {
                            Id = 0,
                            Name = tag.Trim(),
                            Slug = tag.UrlFriendly(),
                            Taxonomy = "tag",
                            Description = "new tag add from post",
                            Count = 1
                        };

                        newTag.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, newTag.Slug, newTag.Id);

                        Database.Session.Save(newTag);
                        var oldTag = Database.Session.Query<Term>().OrderByDescending(ta => ta.Id).FirstOrDefault();
                        if (oldTag != null)
                        {
                            tags.Add(oldTag);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(categories))
            {
                var listCategories = categories.Split(',').Where(t => t != string.Empty).Distinct().ToArray();

                //var catList = listCategories.Select(category => Database.Session.Load<Term>(long.Parse(category))).Where(cat => cat != null).ToList();
                //tags.AddRange(from category in listCategories where category != null select Database.Session.Load<Term>(Convert.ToInt64(category)) into cat where cat != null select cat);
                foreach (var cat in listCategories)
                {
                    var category = Database.Session.Query<Term>().FirstOrDefault(t => t.Id == long.Parse(cat));

                    if (category != null) tags.Add(category);
                }
            }

            post.Category = tags;

            if (post.Category == null) return;

            Database.Session.Update(post);
            Database.Session.Flush();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(PostsForm form)
        {
            if (string.IsNullOrEmpty(Request["categories"]))
            {
                ModelState.AddModelError("categories", "Category is not set");
            }
            if (!ModelState.IsValid)
            {
                return View(new PostsForm
                {
                    Day = DateTime.Now.Day,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    Hour = DateTime.Now.Hour,
                    Minutes = DateTime.Now.Minute
                });
            }

            var post = new Post
            {
                Id = 0,
                CreateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0),
                User = Auth.User,
                Title = form.Title,
                Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly(),
                Excerpt = form.Excerpt,
                Content = form.Content,
                Type = PostType,
                Status = form.Status,
                CommentStatus = "open"
            };

            post.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, post.Slug, post.Id);
            Database.Session.Save(post);

            var postNew = Database.Session.Query<Post>().OrderByDescending(t => t.Id).FirstOrDefault();

            if (postNew == null) return RedirectToAction("Index");

            post.Category = new List<Term>();

            post.CreateKeyValue(postNew.Id, "sticky", Request["Sticky"]);
            post.CreateKeyValue(postNew.Id, "keyword", Request["keyword"]);
            post.CreateKeyValue(postNew.Id, "thumbnail_id", Request["image-choose"]);

            AddCategoriesTagsPost(postNew, Request["ctags"], Request["categories"]);

            TempData["FlashSuccess"] = "Created success!";

            return RedirectToAction("Edit", new { id = postNew.Id });
        }



        public ActionResult Edit(long id)
        {
            //var post = Database.Session.Load<Post>((Int64)id);
            var post = Database.Session.Query<Post>().SingleOrDefault(t => t.Id == id);

            if (post == null) return HttpNotFound();

            ViewBag.Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();

            var stick = string.Empty;

            var metaSticky = post.PostMetas.FirstOrDefault(t => t.PostId == id && t.MetaKey == "sticky");
            if (metaSticky != null)
            {
                stick = metaSticky.MetaValue;
            }
            var keyword = post.PostMetas.FirstOrDefault(t => t.PostId == id && t.MetaKey == "keyword");

            var keyw = string.Empty;

            if (keyword != null)
            {
                keyw = keyword.MetaValue;
            }
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
                Category = post.Category,
                Sticky = stick,
                Keyword = keyw
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(long id, PostsForm form)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null) return HttpNotFound();

            ViewBag.Category = Database.Session.Query<Term>().Where(t => t.Taxonomy == "cat").ToList();

            post.Category = new List<Term>();

            if (!ModelState.IsValid)
            {
                TempData["FlashWarning"] = "Update error!";
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
            }
            post.User = Auth.User;

            post.UpdateAt = new DateTime(form.Year, form.Month, form.Day, form.Hour, form.Minutes, 0, 0);

            post.Title = form.Title;
            post.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Title.UrlFriendly();
            post.Excerpt = form.Excerpt;
            post.Content = form.Content;
            post.Type = PostType;
            post.Status = form.Status;
            post.CommentStatus = "open";
            post.Category = null;

            post.Slug = UniqueSlug.CreateSlug(CheckSlugUnique, post.Slug, post.Id);
            Database.Session.Update(post);

            AddCategoriesTagsPost(post, Request["ctags"], Request["categories"]);

            post.CreateKeyValue(post.Id, "sticky", form.Sticky);
            post.CreateKeyValue(post.Id, "keyword", form.Keyword);
            post.CreateKeyValue(post.Id, "thumbnail_id", Request["image-choose"]);

            TempData["FlashSuccess"] = "Updated success!";
            return RedirectToAction("Edit", new { Id = id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(long id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null) return HttpNotFound();
            Database.Session.Update(post);

            return RedirectToAction("Index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null) return HttpNotFound();

            Database.Session.Delete(post);
            Database.Session.Flush();

            TempData["FlashSuccess"] = "Deleted success!";
            return RedirectToAction("Index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Restore(long id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null) return HttpNotFound();

            Database.Session.Update(post);

            return RedirectToAction("Index");

        }


        /// <summary>
        /// Xoa cac bai viet co id chua trong listItem
        /// </summary>
        public ActionResult DeletePosts()
        {
            var listIdPost = Request["DeletePosts"];

            if (string.IsNullOrEmpty(listIdPost)) return RedirectToAction("Index");

            var arraySlug = listIdPost.Split(',').Where(p => p != string.Empty).Distinct().ToArray();

            if (!arraySlug.Any()) return RedirectToAction("Index");

            foreach (var idPost in arraySlug)
            {
                long id;

                var flag = long.TryParse(idPost, out id);

                if (!flag) continue;

                var post = Database.Session.Load<Post>(id);
                if (post == null) continue;
                Database.Session.Delete(post);
                Database.Session.Flush();
            }

            return RedirectToAction("Index");
        }
    }
}