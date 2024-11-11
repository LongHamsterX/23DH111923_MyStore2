using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace _23DH111923_MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        public string PageNumber { get; set; }
        public string PageSize { get; set; }

        public List<Product> FeaturedProducts { get; set; }

        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}