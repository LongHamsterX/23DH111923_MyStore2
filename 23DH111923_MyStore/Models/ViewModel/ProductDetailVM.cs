using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using PagedList.Mvc;

namespace _23DH111923_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; } // Thông tin sản phẩm

        public int quantity { get; set; } = 1; // Số lượng sản phẩm, mặc định là 1

        // Tính giá trị tạm thời (tổng tiền)
        public decimal estimatedValue => quantity * product.ProductPrice;

        // Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } // Trang hiện tại
        public int PageSize { get; set; } = 3; // Số sản phẩm mỗi trang

        // Danh sách 8 sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // Danh sách 8 sản phẩm bán chạy nhất cùng danh mục
        public PagedList.IPagedList<Product> TopProducts { get; set; }
    }
}