using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
         
            var currentPageIndex = page.HasValue ? page.Value - 1 : 0;

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
            if (string.IsNullOrEmpty(listItem)) return;

            var items = listItem.Split(',').Where(p => p != string.Empty).ToList();

            foreach (var item in items)
            {
                var getImage = Database.Session.Load<Post>(Int64.Parse(item));

                if (getImage == null) continue;

                DeleteFile(getImage.Guid, "~/Uploads/");
                DeleteFile(getImage.Guid, "~/Uploads/thumb/");
                DeleteFile(getImage.Guid, "~/Uploads/medium/");
                DeleteFile(getImage.Guid, "~/Uploads/large/");
            }

            //DeleteList(listItem);
        }

        public void DeleteFile(string file, string folder)
        {
            var location = System.Web.HttpContext.Current.Server.MapPath(folder);

            var fileName = file;

            if (location == null) return;

            var path = Path.Combine(location, fileName);
            FileInfo fileOrg = new FileInfo(path);
            if (fileOrg.Exists)
            {
                fileOrg.Delete();
            }
        }
        private readonly string folderUpload = @"~\Uploads\";
        public void UploadMedia(dynamic numFiles)
        {
            var imageFileName = "";

            
            for (var i = 0; i < numFiles.Count; i++)
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
                var left = 0;
                var top = 0;
                int srcWidth;
                int srcHeight;

                var bitmap = new Bitmap(Width, Height);
                bitmap.SetResolution(72, 72);

                var croppedHeightToWidth = (double)Height / Width;

                var croppedWidthToHeight = (double)Width / Height;

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

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighSpeed;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.CompositingQuality = CompositingQuality.HighSpeed;
                    g.InterpolationMode = InterpolationMode.Low;                    
                    g.DrawImage(
                        image, 
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                        new Rectangle(left, top, srcWidth, srcHeight), 
                        GraphicsUnit.Pixel
                        );
                }

                finalImage = bitmap;                
            }
            catch
            {
                return;
            }

            var folderSave = System.Web.HttpContext.Current.Server.MapPath(folderSaveFile);

            if (folderSave == null) return;

            var pathFinal = Path.Combine(folderSave, file_name);

            var ms = new MemoryStream();
            finalImage.Save(ms, image.RawFormat);
            
            var t = GetCroppedImage(ms.GetBuffer(), new Size(width, height), image.RawFormat);

            var img = new WebImage(t);

            img.Save(pathFinal, img.ImageFormat);

        }
        private byte[] GetCroppedImage(byte[] originalBytes, Size size, ImageFormat format)
        {
            using (var streamOriginal = new MemoryStream(originalBytes))
            using (var imgOriginal = Image.FromStream(streamOriginal))
            {
                //get original width and height of the incoming image
                var originalWidth = imgOriginal.Width; // 1000
                var originalHeight = imgOriginal.Height; // 800

                //get the percentage difference in size of the dimension that will change the least
                var percWidth = ((float)size.Width / (float)originalWidth); // 0.2
                var percHeight = ((float)size.Height / (float)originalHeight); // 0.25
                var percentage = Math.Max(percHeight, percWidth); // 0.25

                //get the ideal width and height for the resize (to the next whole number)
                var width = (int)Math.Max(originalWidth * percentage, size.Width); // 250
                var height = (int)Math.Max(originalHeight * percentage, size.Height); // 200

                //actually resize it
                using (var resizedBmp = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage((Image)resizedBmp))
                    {
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.DrawImage(imgOriginal, 0, 0, width, height);
                    }

                    //work out the coordinates of the top left pixel for cropping
                    var x = (width - size.Width) / 2; // 25
                    var y = (height - size.Height) / 2; // 0

                    //create the cropping rectangle
                    var rectangle = new Rectangle(x, y, size.Width, size.Height); // 25, 0, 200, 200

                    //crop
                    using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        //get the codec needed
                        var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

                        //make a paramater to adjust quality
                        var codecParams = new EncoderParameters(1);

                        //reduce to quality of 80 (from range of 0 (max compression) to 100 (no compression))
                        codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

                        //save to the memorystream - convert it to an array and send it back as a byte[]
                        croppedBmp.Save(ms, imgCodec, codecParams);
                        return ms.ToArray();
                    }
                }
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