using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;
using NHibernate.Linq;
using static System.Int32;
using static System.String;

namespace Blog.Infrastructure
{
    public static class UniqueSlug
    {
        public delegate bool SlugUnique(long id, string postName);

        public static string CreateSlug(SlugUnique method, string slug, long id)
        {
            var level = 1;
            while (true)
            {
                //nếu chưa có trong csdl thì trả về ngay.
                if (false == method(id, slug)) return slug;

                var postNameArray = slug.Split('-');
                int j;

                //tách lấy số phiên bản - mảng post_name có chỉ số cuối cùng
                var flagId = TryParse(postNameArray[postNameArray.Length - 1], out j);

                if (flagId)
                {
                    //Nếu tách thành công tức là post_name dạng: trang-chu-[n]; n=1,2,3,...                    
                    slug = Join("-", postNameArray, 0, postNameArray.Length - 1) + "-" + level;
                }
                else
                {
                    slug = slug + "-" + level;
                }
                
                level++;
            }
        }
    }
}