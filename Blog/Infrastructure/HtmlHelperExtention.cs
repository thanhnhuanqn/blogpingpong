using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Blog.Infrastructure
{
    public static class HtmlHelperExtention
    {
        public static string CreateAction<T>(this HtmlHelper helper, IEnumerable<T> list, Func<T, object> action, Func<T, object> fxns)
        {
            StringBuilder sb = new StringBuilder();

            string a = string.Empty;

            foreach (var item in list)
            {                                
                    //sb.Append("<a href=\"" +  + "\">");
                    sb.Append(fxns(item));
                    sb.Append("</a>");
                    sb.Append("--");                
            }

            return sb.ToString();
        }
    }
}