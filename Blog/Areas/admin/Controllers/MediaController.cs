using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Blog.Areas.admin.ViewModels;
using Blog.Infrastructure;
using Blog.Models;
using CKFinder.Connector;
using NHibernate.Linq;
using Blog.Module.Paging;

namespace Blog.Areas.admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("Media")]
    public class MediaController : Controller
    {
        private string PostType = "Image";        
        private const int DefaultPageSize = 15;
        // GET: admin/Media
        public ActionResult Index(int? page = 1)
        {          
            var files = Request.Files;
            if (files != null)
            {
                UploadMedia(files);
            }

            var listItem = Request["actionDelete"];

            if (listItem != null)
            {
                DeleteListImage(listItem);
            }

            var baseQuery = Database.Session.Query<Post>().Where(t=>t.Type== PostType).OrderByDescending(c => c.CreateAt);
         
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(baseQuery.ToPagedList(currentPageIndex, DefaultPageSize));

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>((Int64)id);

            if (post == null) return HttpNotFound();
            try
            {
                DeleteFile(post.Guid, "~/Uploads/");
                DeleteFile(post.Guid, "~/Uploads/thumb/");
                DeleteFile(post.Guid, "~/Uploads/medium/");
                DeleteFile(post.Guid, "~/Uploads/large/");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Database.Session.Delete(post);
            Database.Session.Flush();
            return RedirectToAction("Index");

        }
        public void DeleteListImage(string listItem)
        {
            if (!string.IsNullOrEmpty(listItem))
            {
                var items = listItem.Split(',').Where(p => p != string.Empty).ToList();
                foreach (var item in items)
                {
                    var getImage = Database.Session.Load<Post>(Int64.Parse(item));

                    if (getImage != null)
                    {
                        DeleteFile(getImage.Guid, "~/Uploads/");
                        DeleteFile(getImage.Guid, "~/Uploads/thumb/");
                        DeleteFile(getImage.Guid, "~/Uploads/medium/");
                        DeleteFile(getImage.Guid, "~/Uploads/large/");
                    }
                }

                //DeleteList(listItem);
            }
        }

        public void DeleteFile(string file, string folder)
        {
            var location = System.Web.HttpContext.Current.Server.MapPath(folder);

            var fileName = file;

            if (location != null)
            {
                var path = Path.Combine(location, fileName);
                FileInfo fileOrg = new FileInfo(path);
                if (fileOrg.Exists)
                {
                    fileOrg.Delete();
                }
            }
        }
        private readonly string folderUpload = @"~\Uploads\";
        public void UploadMedia(dynamic numFiles)
        {
            var imageFileName = "";

            
            for (int i = 0; i < numFiles.Count; i++)
            {
                var file = numFiles[i];
                if (file.ContentLength > 0)
                {
                    var originalImage = new WebImage(file.InputStream);

                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    if (fileNameWithoutExtension != null)
                        imageFileName = Guid.NewGuid() + "_" +
                                        fileNameWithoutExtension.Trim();

                    
                    originalImage.Save(folderUpload + imageFileName.UrlFriendly());
                    
                    var extension = Path.GetExtension(originalImage.FileName);

                    if (extension == null || fileNameWithoutExtension == null) continue;

                    var imageFile = imageFileName.UrlFriendly() + extension;

                    var image = new Post
                    {
                        User = Database.Session.Load<User>(1),
                        Title = fileNameWithoutExtension,
                        Slug = imageFileName,                                
                        Content = "Image upload",
                        Status = "inherit",
                        Type = PostType,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        Guid = imageFile,
                        CommentStatus = "Open",                                
                    };
                    Database.Session.Save(image);
                    //Thumbs
                    CreateImage(300, 300, file_name: imageFile, folderSaveFile: @"~\Uploads\Thumb\");
                    CreateImage(600, 600, file_name: imageFile, folderSaveFile: @"~\Uploads\Medium\");
                    CreateImage(1024, 768, file_name: imageFile, folderSaveFile: @"~\Uploads\Large\");
                }
            }

        }


        public void CreateImage(int width, int height, string file_name, string folderSaveFile)
        {
            var Width = 0;
            var Height = 0;
            try
            {
                Width = Convert.ToInt32(width);
                Height = Convert.ToInt32(height);
            }
            catch
            {
                return;
            }
            string fileName = file_name;
            string folder = "~/Uploads/";

            var location = System.Web.HttpContext.Current.Server.MapPath(folder);
            var path = Path.Combine(location, fileName);
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);

            System.Drawing.Image finalImage;

            try
            {
                int left = 0;
                int top = 0;
                int srcWidth;
                int srcHeight;

                var bitmap = new System.Drawing.Bitmap(Width, Height);

                double croppedHeightToWidth = (double)Height / Width;

                double croppedWidthToHeight = (double)Width / Height;

                if (image.Width > image.Height)
                {
                    srcWidth = (int)(Math.Round(image.Height * croppedWidthToHeight));
                    if (srcWidth < image.Width)
                    {
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                    else
                    {
                        srcHeight = (int)Math.Round(image.Height * ((double)image.Width / srcWidth));
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                }
                else
                {
                    srcHeight = (int)(Math.Round(image.Width * croppedHeightToWidth));
                    if (srcHeight < image.Height)
                    {
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                    else
                    {
                        srcWidth = (int)Math.Round(image.Width * ((double)image.Height / srcHeight));
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                }

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), new System.Drawing.Rectangle(left, top, srcWidth, srcHeight), System.Drawing.GraphicsUnit.Pixel);
                }

                finalImage = bitmap;
            }
            catch
            {
                return;
            }

            var folderSave = System.Web.HttpContext.Current.Server.MapPath(folderSaveFile);
            if (folderSave != null)
            {
                var pathFinal = Path.Combine(folderSave, file_name);

                MemoryStream ms = new MemoryStream();
                finalImage.Save(ms, image.RawFormat);
                var img = new WebImage(ms.GetBuffer());
                img.Resize(Width, Height);
                img.Save(pathFinal, img.ImageFormat);
            }
        }

        [HttpPost]
        public ActionResult Resize(FormCollection data)
        {
            var largeSizeW = int.Parse(Request["large_size_w"]);
            var largeSizeH = int.Parse(Request["large_size_h"]);

            foreach (var file in GetAllPagesInFolder("~/uploads", "*.*"))
            {                
                //Thumbs]
                var fileName = file.Split('/').Reverse().ToList();

                CreateImage(width: largeSizeW, height: largeSizeH, file_name: fileName[0], folderSaveFile: @"~\uploads\tmp\");                
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Đọc tất cả các file trong một thư mục được chỉ định.
        /// </summary>
        /// <param name="virtualPath">đường dẫn tới thư mục</param>
        /// <param name="searchPattern">tên file tìm kiếm</param>
        /// <returns>tất cả các file trong thư mục đó.</returns>

        public static string[] GetAllPagesInFolder(string virtualPath, string searchPattern)
        {
            string physicalPath = System.Web.HttpContext.Current.Server.MapPath(virtualPath);

            // childrenFiles are physical paths. We need to convert them into virtual paths
            string[] childrenFiles = Directory.EnumerateFiles(physicalPath, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
            string[] childrenVirtualPaths = new string[childrenFiles.Length];

            for (int i = 0; i < childrenFiles.Length; ++i)
            {
                // convert physical path to virtual path
                childrenVirtualPaths[i] = childrenFiles[i].Replace(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, "~/").Replace(Path.DirectorySeparatorChar, '/');
            }

            return childrenVirtualPaths;
        }


        /// <summary>
        /// Xoa cac bai viet co id chua trong listItem
        /// </summary>
        public ActionResult DeleteImages()
        {
            var listIdPost = Request["DeleteImages"];

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

                DeleteFile(post.Guid, "~/Uploads/");
                DeleteFile(post.Guid, "~/Uploads/thumb/");
                DeleteFile(post.Guid, "~/Uploads/medium/");
                DeleteFile(post.Guid, "~/Uploads/large/");
                DeleteFile(post.Guid, "~/Uploads/tmp/");

                Database.Session.Delete(post);
                Database.Session.Flush();
            }

            return RedirectToAction("Index");
        }
    }
}