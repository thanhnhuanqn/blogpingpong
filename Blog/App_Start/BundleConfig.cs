﻿using System.Web;
using System.Web.Optimization;

namespace Blog
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/scrollSpeed").Include(
                        "~/Scripts/jQuery.scrollSpeed.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                      "~/Scripts/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/timeago").Include(
                      "~/Scripts/jquery.timeago.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            
            bundles.Add(new StyleBundle("~/Content/admin").Include(
                "~/Content/bootstrap.css",
                "~/Areas/admin/Contents/admin.css"));

            bundles.Add(new ScriptBundle("~/bundles/froms").Include(
                "~/Areas/admin/Scripts/forms.js"));
            
            //use
            bundles.Add(new StyleBundle("~/Content/pingpong")                
                .Include("~/Content/Style.css")
                .Include("~/Content/font-awesome.css")
                );            

            BundleTable.EnableOptimizations = true;
        }
        
    }
}
