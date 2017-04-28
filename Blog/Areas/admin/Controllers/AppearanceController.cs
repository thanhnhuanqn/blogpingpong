using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using CKFinder.Connector;
using NHibernate.Linq;

namespace Blog.Areas.admin.Controllers
{
    [SelectedTab("Appearance")]
    public class AppearanceController : Controller
    {
        // GET: admin/Appearance
        public ActionResult Index()
        {
            return View(new Appearence());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateMenu()
        {
            var menuName = Request["menu-name"];
            if (menuName == null) return RedirectToAction("Index");

            var nav = new Term
            {
                Name = menuName,
                Slug = menuName.UrlFriendly(),
                Description = "Menu",
                Taxonomy = "menu",
                Parent =  0,
                Count = 0
            };                
            try
            {
                Database.Session.Save(nav);
                TempData["FlashSuccess"] = "Created menu success!";
            }
            catch (Exception ex)
            {
                TempData["FlashWarning"] = "Create menu error!" + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Menu(int id)
        {
            var menu = Database.Session.Load<Term>((long)id);

            if (menu == null) return HttpNotFound();

            return View(new Nav(menu));
        }
        public ActionResult EditMenu(int id)
        {
            var menu = Database.Session.Load<Term>((long)id);

            if (menu == null) return HttpNotFound();

            return View(menu);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditMenu(int id, Term form)
        {
            var menu = Database.Session.Load<Term>((long)id);

            if (menu == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {                
                return View(form);
            }

            menu.Name = form.Name;
            menu.Slug = !string.IsNullOrEmpty(form.Slug) ? form.Slug.UrlFriendly() : form.Name.UrlFriendly();
            menu.Description = form.Description;
            menu.Parent = 0;

            Database.Session.Update(menu);
            Database.Session.Flush();

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteMenu(int id)
        {
            var menu = Database.Session.Load<Term>((long)id);

            if (menu == null) return HttpNotFound();

            Database.Session.Delete(menu);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }

        public ActionResult Editor()
        {
            return null;
        }

     
        public ActionResult DeleteMenuItem(string id)
        {
            var ids = id.Split('-');
            var post = Database.Session.Load<Post>(long.Parse(ids[0]));

            if (post == null) return HttpNotFound();

            Database.Session.Delete(post);
            Database.Session.Flush();

            TempData["FlashSuccess"] = "Deleted success!";
            var idMenu = Request["idMenu"].AsInt();
            
            return RedirectToAction("Menu", new {id = long.Parse(ids[1])});
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateItemsNav(int id)
        {
            var menu = Database.Session.Load<Term>((long)id);

            if (menu == null) return HttpNotFound();

            var menuItemName = Request["menu-item-name"] ?? "";
            var menuItemOrder = Request["menu-item-order"] ?? "";
            var menuItemId = Request["menu-item-id"] ?? "";
            var navItemParent = Request["nav-item-parent"]??"";

            if (string.IsNullOrEmpty(menuItemId) ||
                string.IsNullOrEmpty(menuItemOrder) ||
                string.IsNullOrEmpty(menuItemId) ||
                string.IsNullOrEmpty(navItemParent))
            {
                TempData["FlashWarning"] = "No records updated";
                return RedirectToAction("Menu", new { id = id });
            }
            var ids = menuItemId.Split(',');
            var names = menuItemName.Split(',');
            var orders = menuItemOrder.Split(',');
            var parents= navItemParent.Split(',');

            var i = 0;
            foreach (var item in ids)
            {                
                var post = Database.Session.Load<Post>(long.Parse(item));

                if (post != null)
                {
                    post.Title = names[i];
                    post.MenuOrder = Convert.ToInt32(orders[i]);
                    post.Parent = Convert.ToInt32(parents[i]);
                    Database.Session.Update(post);
                    Database.Session.Flush();
                }
                i++;
            }

            TempData["FlashSuccess"] = "Updated success";
            return RedirectToAction("Menu", new {id = id});
        }


        public ActionResult Themes()
        {
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddToMenu(Appearence form, int id = 0)
        {
            var nav = Database.Session.Load<Term>((long)id);
            if (nav == null) return HttpNotFound();                                    
                        
            AddPageToMenu(Request["menu-item-page"], nav);
            AddCategoriesToMenu(Request["menu-item-cat"], nav);
            AddCustomLinkToMenu(Request["menu-item-url"], Request["menu-item-url-title"], nav);
            
            return RedirectToAction("Menu", new {id = nav.Id});            
        }


        private static void AddPageToMenu(string itemsMenu, Term nav)
        {                                 
            if (string.IsNullOrEmpty(itemsMenu)) return;            
                        
            var pageArray = itemsMenu.Split(',');

            foreach (var item in pageArray)
            {                        
                var pageMenu = Database.Session.Load<Post>(Convert.ToInt64(item));

                if (pageMenu == null) continue;
                var page = new Post
                {
                    Content = "Add page " + pageMenu.Title + " to menu  " + nav.Name,
                    CreateAt = DateTime.Now,
                    Type = "nav_menu_item",
                    Status = "publish",
                    Title = pageMenu.Title,
                    Slug = pageMenu.Slug.UrlFriendly() + pageMenu.Id.ToString(),
                    User = Database.Session.Load<User>(1),
                    Parent = 0,
                    MenuOrder = 0,
                    CommentCount = 0,
                    CommentStatus = "close"
                };

                Database.Session.Save(page);

                var pageItem = Database.Session.Query<Post>().OrderByDescending(t => t.Id).FirstOrDefault();

                if (pageItem == null) continue;

                PostsController.AddCategoriesTagsPost(post: pageItem, listTag:null, categories: nav.Id.ToString());
                pageItem.CreateKeyValue(pageItem.Id, "_item_menu_type", "page");                                        
                pageItem.CreateKeyValue(pageItem.Id, "_item_menu_post_parent", pageMenu.Id.ToString());
            }                            
        }



        private static void AddCategoriesToMenu(string itemsCat, Term nav)
        {            
            if (string.IsNullOrEmpty(itemsCat)) return;         

            var pageArray = itemsCat.Split(',');

            foreach (var item in pageArray)
            {
                var catMenu = Database.Session.Load<Term>(Convert.ToInt64(item));

                if (catMenu == null) continue;

                var page = new Post
                {
                    Content = "Add page " + catMenu.Name + " to menu  " + nav.Name,
                    CreateAt = DateTime.Now,
                    Type = "nav_menu_item",
                    Status = "publish",
                    Title = catMenu.Name,
                    Slug = catMenu.Slug.UrlFriendly() + catMenu.Id.ToString(),
                    User = Database.Session.Load<User>(1),
                    Parent = 0,
                    MenuOrder = 0,
                    CommentCount = 0,
                    CommentStatus = "close"
                };

                Database.Session.Save(page);

                var pageItem = Database.Session.Query<Post>().OrderByDescending(t => t.Id).FirstOrDefault();

                if (pageItem == null) continue;

                PostsController.AddCategoriesTagsPost(post: pageItem, listTag: null, categories: nav.Id.ToString());
                pageItem.CreateKeyValue(pageItem.Id, "_item_menu_type", "cat");
                pageItem.CreateKeyValue(pageItem.Id, "_item_menu_post_parent", catMenu.Id.ToString());
            }
        }


        private static void AddCustomLinkToMenu(string url, string title, Term nav)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title)) return;
            
            var pageArray = url.Split(',');

            foreach (var item in pageArray)
            {                
                var customLink = new Post
                {
                    Content = "Add custom menu to menu " + nav.Name,
                    CreateAt = DateTime.Now,
                    Type = "nav_menu_item",
                    Status = "publish",
                    Title = title,
                    Slug = title.UrlFriendly(),
                    User = Database.Session.Load<User>(1),
                    Parent = 0,
                    MenuOrder = 0,
                    CommentCount = 0,
                    CommentStatus = "close"
                };

                Database.Session.Save(customLink);
                
                var customItem = Database.Session.Query<Post>().OrderByDescending(t => t.Id).FirstOrDefault();

                if (customItem == null) continue;

                PostsController.AddCategoriesTagsPost(post: customItem, listTag: null, categories: nav.Id.ToString());
                customItem.CreateKeyValue(customItem.Id, "_item_menu_type", "link");
                customItem.CreateKeyValue(customItem.Id, "_item_menu_url", customItem.Id.ToString());
            }
        }
    }
}