using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Blog.Models;
using System.Web.Mvc;
using NHibernate.Linq;

namespace Blog.Areas.admin.ViewModels
{
    public class ProductsData
    {
        public enum Data
        {
            Simple,
            Group,
            External,
            Variable
        }
    }
    public class Product
    {

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }

        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public int CommentCount { get; set; }
        public string Sticky { get; set; }

        [Required(ErrorMessage = "Product name cannot empty"), MaxLength(255)]
        [DisplayName(displayName:"Tên sản phẩm")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [DataType(DataType.MultilineText), AllowHtml]
        [DisplayName(displayName: "Mô tả")]
        public string ShortDescription { get; set; }
        [DataType(DataType.MultilineText), AllowHtml]
        [Required(ErrorMessage = "Product description cannot empty")]
        [DisplayName(displayName: "Thông tin chi tiết")]
        public string Description { get; set; }
        
        //General
        public string ProductData { get; set; }
        [DataType(DataType.Custom)]
        [DisplayName(displayName: "Giá bán")]
        public decimal RegularPrice { get; set; }
        [DataType(DataType.Custom)]
        [DisplayName(displayName: "Giá sale")]
        public decimal SalePrice { get; set; }
        public DateTime? SalePriceDatesFrom { get; set; }
        public DateTime? SalePriceDatesTo { get; set; }
        public string TaxStatus { get; set; }
        public string TaxClass { get; set; }


        //Inventory
        [DisplayName(displayName: "Mã hàng (SKU)")]
        public string Sku { get; set; }
        public bool ManageStock { get; set; }
        public int StockQuantity { get; set; }
        public string Backorders { get; set; }
        public string StockStatus { get; set; }
        public bool SoldIndividually { get; set; }
        
        //Shippng
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string ProductShippingClass { get; set; }
        
        //Linked Products
        public string UpsellIds { get; set; }
        public string CrosssellIds { get; set; }
                
        public string PurchaseNote { get; set; }
        public int MenuOrder { get; set; }
        public bool EnableReviews { get; set; }
        public Post ImageProduct { get; set; }               

        public virtual User User { get; set; }
        public List<Term> Categories { get; set; }
        public List<Term> Tags { get; set; }
        public List<Term> Attributes { get; set; }
        public List<Post> ProductGallery { get; set; }

        public Product()
        {            
            var date = DateTime.Now;
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
            Hour = date.Hour;
            Minutes = date.Minute;

            ProductData = "Simple";
            SalePrice = 0;
            ManageStock = false;
            StockQuantity = 0;
            Backorders = "Do not Allow";
            StockStatus = "In stock";
            SoldIndividually = false;

            Type = "product";
            CommentCount = 0;
            Weight = 0;

            MenuOrder = 0;
            Status = "publish";
            EnableReviews = true;

            Tags = Database.Session.Query<Term>().Where(t => t.Taxonomy == "product_tag").ToList();
            Categories = Database.Session.Query<Term>().Where(t => t.Taxonomy == "product_cat").ToList();
            Attributes = Database.Session.Query<Term>().Where(t => t.Taxonomy == "product_attr").ToList();
            ProductGallery = Database.Session.Query<Post>().Where(t => t.Type == "image").ToList();

            Created = SetupDateTime(Year, Month, Day, Hour, Minutes);
            if (Id > 0)
            {
                Updated = SetupDateTime(Year, Month, Day, Hour, Minutes);
            }
        }

        public DateTime SetupDateTime(int year, int month, int day, int hour, int minutes)
        {
            if (day > DateTime.DaysInMonth(year, month) || day < 0)
            {
                day = DateTime.DaysInMonth(year, month);
            }

            if (minutes > 59 || minutes < 0)
            {
                minutes = 0;
            }

            if (hour > 23 || hour < 0)
            {
                hour = 0;
            }

            return new DateTime(year, month, day, hour, minutes, 0, 0);

        }

    }
}