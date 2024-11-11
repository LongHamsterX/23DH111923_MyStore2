using _23DH111923_MyStore.Models.ViewModel;
using _23DH111923_MyStore.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace _23DH111923_MyStore.Models.ViewModel
{
    public class CheckoutVM // Lưu thông tin cho form Checkout
    {
        public List<CartItem> CartItem { get; set; }
        public int CustomerID { get; set; }
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Tổng giá trị")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Phương thức giao hàng")]
        public string ShippingMethod { get; set; }
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }
        public string Username { get; set; }
        // Các thuộc tính khác của đơn hàng
        public List<OrderDetail> OrderDetails { get; set; }
    }
}