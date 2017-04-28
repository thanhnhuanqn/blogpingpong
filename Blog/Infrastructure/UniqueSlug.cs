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
            
            //nếu chưa có trong csdl thì trả về ngay.
            if (false == method(id, slug)) return slug;

            var postNameArray = slug.Split('-');
            int j;

            //tách lấy số phiên bản - mảng post_name có chỉ số cuối cùng
            var flagId = TryParse(postNameArray[postNameArray.Length - 1], out j);

            //Nếu tách thành công tức là post_name dạng: trang-chu-[n]; n=1,2,3,...
            var joinToPostName = flagId ? Join("-", postNameArray, 0, postNameArray.Length - 1) : slug;

            //Giới hạn 999 phiên bản - có thể dùng đệ qui để bỏ giới hạn.
            for (var i = 1; i < 1000; i++)
            {
                var flag = method(id, joinToPostName + "-" + i);

                //Nếu không tồn lại post_name này trong csdl
                if (false == flag)
                {
                    return joinToPostName + "-" + i;
                }                
            }

            return slug;
        }
        
    }
}