using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Blog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.Configure();
            MvcHandler.DisableMvcResponseHeader = true;
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");           //Remove Server Header  
            Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Database.OpenSession();

            bool allowCompression = false;
            bool.TryParse(ConfigurationManager.AppSettings["Compression"], out allowCompression);

            if (allowCompression)
            {
                // Implement HTTP compression
                HttpApplication app = (HttpApplication) sender;

                // Retrieve accepted encodings
                string encodings = app.Request.Headers.Get("Accept-Encoding");
                if (encodings != null)
                {
                    // Check the browser accepts deflate or gzip (deflate takes preference)
                    encodings = encodings.ToLower();
                    if (encodings.Contains("deflate"))
                    {
                        app.Response.Filter = new DeflateStream(app.Response.Filter, CompressionMode.Compress);
                        app.Response.AppendHeader("Content-Encoding", "deflate");
                    }
                    else if (encodings.Contains("gzip"))
                    {
                        app.Response.Filter = new GZipStream(app.Response.Filter, CompressionMode.Compress);
                        app.Response.AppendHeader("Content-Encoding", "gzip");
                    }
                }
            }
        }

        protected void Application_EndRequest()
        {            
            Database.CloseSession();
        }
    }
}
