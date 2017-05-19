using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ImageController : Controller
    {
        public void Index(string imageFile)
        {
            if (string.IsNullOrEmpty(imageFile)) return;

            imageFile = imageFile.Trim();
            var tach = imageFile.Split('?');
            var size = tach[1];
            imageFile = tach[0];
            var tmp = size.Split('x').ToList();

            var width = 0;
            var height = 0;
            try
            {
                width = Math.Abs(Convert.ToInt32(tmp.ElementAt(0)));
                height = Math.Abs(Convert.ToInt32(tmp.ElementAt(1)));

                if (width > 4000)
                {
                    width = 1024;
                }
                if (height > 4000)
                {
                    height = 1024;
                }

            }
            catch
            {
                return;
            }           

            var location = Server.MapPath(@"~/Uploads/Tmp/");
            var fileSave = width + "x" + height + "_" + imageFile;
            var path = Path.Combine(location, fileSave);
            var f = new FileInfo(path);

            if (!f.Exists)
            {
                _createImage(width, height, imageFile, "~/Uploads/Tmp/");
            }
            
            location = Server.MapPath("~/Uploads/Tmp/");
            path = Path.Combine(location, fileSave);
            f = new FileInfo(path);

            if (!f.Exists) return;

            var img = new WebImage(path);

            img.Write();
        }

        private void _createImage(int width, int height, string file_name, string folderSaveFile)
        {
            if (file_name == null) throw new ArgumentNullException(nameof(file_name));

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
            var fileName = file_name;
            const string folder = "~/Uploads/";

            var location = HttpContext.Server.MapPath(folder);

            var path = Path.Combine(location, fileName);

            var f = new FileInfo(path);
            if (!f.Exists)
            {
                return;
            }
            var image = System.Drawing.Image.FromFile(path);

            System.Drawing.Image finalImage;

            try
            {
                var left = 0;
                var top = 0;
                var srcWidth = Width;
                var srcHeight = Height;

                var bitmap = new System.Drawing.Bitmap(Width, Height);

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

            var fileSave = Width + "x" + Height + "_" + fileName;

            var folderSave = HttpContext.Server.MapPath(folderSaveFile);
            var pathFinal = Path.Combine(folderSave, fileSave);

            var ms = new MemoryStream();
            finalImage.Save(ms, image.RawFormat);
            var img = new WebImage(ms.GetBuffer());
            img.Resize(Width, Height);
            img.Save(pathFinal, img.ImageFormat);

        }

    }
}