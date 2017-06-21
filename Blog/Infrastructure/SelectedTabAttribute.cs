using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Blog.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SelectedTabAttribute : ActionFilterAttribute
    {
        private readonly string _selectedTab;

        public SelectedTabAttribute(string selectedTab)
        {
            _selectedTab = selectedTab;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.SelectedTab = _selectedTab;
        }
    }


    //public class GZipOrDeflateAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting
    //         (ActionExecutingContext filterContext)
    //    {
    //        string acceptencoding = filterContext.HttpContext.
    //                                Request.Headers["Accept-Encoding"];

    //        if (!string.IsNullOrEmpty(acceptencoding))
    //        {
    //            acceptencoding = acceptencoding.ToLower();
    //            var response = filterContext.HttpContext.Response;
    //            if (acceptencoding.Contains("gzip"))
    //            {
    //                response.AppendHeader("Content-Encoding", "gzip");
    //                response.Filter = new GZipStream(response.Filter,
    //                                      CompressionMode.Compress);
    //            }
    //            else if (acceptencoding.Contains("deflate"))
    //            {
    //                response.AppendHeader("Content-Encoding", "deflate");
    //                response.Filter = new DeflateStream(response.Filter,
    //                                  CompressionMode.Compress);
    //            }
    //        }
    //    }
    //}

    //public class CompressFilter : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        bool allowCompression = false;
    //        bool.TryParse(ConfigurationManager.AppSettings["Compression"], out allowCompression);

    //        if (allowCompression)
    //        {
    //            HttpRequestBase request = filterContext.HttpContext.Request;

    //            string acceptEncoding = request.Headers["Accept-Encoding"];

    //            if (string.IsNullOrEmpty(acceptEncoding)) return;

    //            acceptEncoding = acceptEncoding.ToUpperInvariant();

    //            HttpResponseBase response = filterContext.HttpContext.Response;

    //            if (acceptEncoding.Contains("GZIP"))
    //            {
    //                response.AppendHeader("Content-encoding", "gzip");
    //                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
    //            }
    //            else if (acceptEncoding.Contains("DEFLATE"))
    //            {
    //                response.AppendHeader("Content-encoding", "deflate");
    //                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
    //            }
    //        }
    //    }
    //}

    //public class ETagAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response, filterContext.RequestContext.HttpContext.Request);
    //    }
    //}

    //public class ETagFilter : MemoryStream
    //{
    //    private HttpResponseBase _response = null;
    //    private HttpRequestBase _request;
    //    private Stream _filter = null;

    //    public ETagFilter(HttpResponseBase response, HttpRequestBase request)
    //    {
    //        _response = response;
    //        _request = request;
    //        _filter = response.Filter;
    //    }

    //    private string GetToken(Stream stream)
    //    {
    //        var checksum = new byte[0];
    //        checksum = MD5.Create().ComputeHash(stream);
    //        return Convert.ToBase64String(checksum, 0, checksum.Length);
    //    }

    //    public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        var data = new byte[count];

    //        Buffer.BlockCopy(buffer, offset, data, 0, count);

    //        var token = GetToken(new MemoryStream(data));
    //        var clientToken = _request.Headers["If-None-Match"];

    //        if (token != clientToken)
    //        {
    //            _response.AddHeader("ETag", token);
    //            _filter.Write(data, 0, count);
    //        }
    //        else
    //        {
    //            _response.SuppressContent = true;
    //            _response.StatusCode = 304;
    //            _response.StatusDescription = "Not Modified";
    //            _response.AddHeader("Content-Length", "0");
    //        }
    //    }
    //}
}