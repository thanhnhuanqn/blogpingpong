using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Blog.Infrastructure
{
    public  static class StringExtensions
    {
        /// <summary>
        /// Loại bỏ các ký tự có đấu ra khỏi một post_name hay một term_name
        /// </summary>
        /// <param name="title">Chuổi cần định dạng lại</param>
        /// <returns>Một chuổi không chứa các ký tự có dấu</returns>
        private static string _removeUnicode(string title)
        {
            var a = new char[] { 'à', 'á', 'ạ', 'ả', 'ã', 'â', 'ầ', 'ấ', 'ậ', 'ẩ', 'ẫ', 'ă', 'ằ', 'ắ', 'ặ', 'ẳ', 'ẵ' };
            title = a.Aggregate(title, (current, item) => current.Replace(item, 'a'));

            //e
            var e = new char[] { 'è', 'é', 'ẹ', 'ẻ', 'ẽ', 'ê', 'ề', 'ế', 'ệ', 'ể', 'ễ' };
            title = e.Aggregate(title, (current, item) => current.Replace(item, 'e'));
            
            //o
            var o = new char[] { 'ò', 'ó', 'ọ', 'ỏ', 'õ', 'ô', 'ồ', 'ố', 'ộ', 'ổ', 'ỗ', 'ơ', 'ờ', 'ớ', 'ợ', 'ở', 'ỡ' };
            title = o.Aggregate(title, (current, item) => current.Replace(item, 'o'));
            
            //i
            var i = new char[] { 'ì', 'í', 'ị', 'ỉ', 'ĩ' };
            title = i.Aggregate(title, (current, item) => current.Replace(item, 'i'));
            
            //u
            var u = new char[] { 'ù', 'ú', 'ụ', 'ủ', 'ũ', 'ư', 'ừ', 'ứ', 'ự', 'ử', 'ữ' };
            title = u.Aggregate(title, (current, item) => current.Replace(item, 'u'));
            
            //y
            var y = new char[] { 'ỳ', 'ý', 'ỵ', 'ỷ', 'ỹ' };
            title = y.Aggregate(title, (current, item) => current.Replace(item, 'y'));
            
            //d
            title = title.Replace('đ', 'd');

            return title;
        }

        /// <summary>
        /// Tạo đường dẫn thân thiện 
        /// </summary>
        /// <param name="title">chuổi đường dẫn cần định dạng lại</param>
        /// <returns>Chuổi đã được định dạng</returns>
        public static string UrlFriendly(this string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                // make it all lower case
                title = title.ToLower();

                //xoá bó các ký tự có đấu có trong chuổi - đường đẫn
                title = _removeUnicode(title);

                // remove entities
                title = Regex.Replace(title, @"&\w+;", "");

                // remove anything that is not letters, numbers, dash, or space
                title = Regex.Replace(title, @"[^a-z0-9\-\s]", "");

                // replace spaces
                title = title.Replace(' ', '-');

                // collapse dashes
                title = Regex.Replace(title, @"-{2,}", "-");

                // trim excessive dashes at the beginning
                title = title.TrimStart(new[] { '-' });

                // if it's too long, clip it
                if (title.Length > 160)
                    title = title.Substring(0, 159);

                // remove trailing dashes
                title = title.TrimEnd(new[] { '-' });
            }

            return title;

        }

        public static string NgayVietBai(this DateTime date)
        {
            var ngay = date.ToString("dddd, dd MMMMM yyyy",CultureInfo.InvariantCulture);

            ngay = ngay.Replace("Monday", "T2");
            ngay = ngay.Replace("Tuesday", "T3");
            ngay = ngay.Replace("Wednesday", "T4");
            ngay = ngay.Replace("Thursday", "T5");
            ngay = ngay.Replace("Friday", "T6");
            ngay = ngay.Replace("Saturday", "T7");
            ngay = ngay.Replace("Sunday", "CN");

            ngay = ngay.Replace("January", "Tháng Giêng");
            ngay = ngay.Replace("February", "Tháng Hai");
            ngay = ngay.Replace("March", "Tháng Ba");
            ngay = ngay.Replace("April", "Tháng Tư");
            ngay = ngay.Replace("May", "Tháng Năm");
            ngay = ngay.Replace("June", "Tháng Sáu");
            ngay = ngay.Replace("July", "Tháng Bảy");
            ngay = ngay.Replace("August", "Tháng Tám");
            ngay = ngay.Replace("September", "Tháng Chín");
            ngay = ngay.Replace("October", "Tháng 10");
            ngay = ngay.Replace("November", "Tháng 11");
            ngay = ngay.Replace("December", "Tháng 12");

            return ngay;
        }
    }
}