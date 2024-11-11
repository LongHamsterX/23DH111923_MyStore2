using _23DH111923_MyStore.Models.ViewModel;
using _23DH111923_MyStore.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Linq;

namespace _23DH111923_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            // Kiểm tra giỏ hàng trong session, nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Xác thực người dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới trang Đăng nhập
            var user = db.User.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang Đăng nhập
            // Nếu có rồi thì lấy địa chỉ của khách hàng và gán vào ShippingAddress của CheckoutVM
            var customer = db.Customer.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo đối tượng CheckoutVM để hiển thị thông tin cho form Checkout
            var model = new CheckoutVM
            {
                CartItem = cart, // Lấy danh sách các sản phẩm trong giỏ hàng
                TotalAmount = cart.Sum(item => item.TotalPrice), // Tổng giá trị của các mặt hàng trong giỏ hàng
                OrderDate = DateTime.Now, // Mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress, // Lấy địa chỉ mặc định từ bảng Customer
                CustomerID = customer.CustomerID, // Lấy mã khách hàng từ bảng Customer
                Username = customer.Username // Lấy tên đăng nhập từ bảng Customer
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model, Order order)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra giỏ hàng
                var cart = Session["Cart"] as List<CartItem>;
                if (cart == null || !cart.Any())
                {
                    return RedirectToAction("Index", "Home");
                }

                // Kiểm tra đăng nhập
                var user = db.User.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra thông tin khách hàng
                var customer = db.Customer.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Xác định trạng thái thanh toán dựa trên phương thức thanh toán
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt":
                        paymentStatus = "Thanh toán tiền mặt";
                        break;
                    case "Paypal":
                        paymentStatus = "Thanh toán Paypal";
                        break;
                    // ... các trường hợp khác
                    default:
                        paymentStatus = "Chưa thanh toán";
                        break;
                }

                // Tạo đơn hàng mới và chi tiết đơn hàng
                var order1 = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetail = cart.Select(item => new OrderDetail
                    {
                        Quantity = item.Quantity,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                var orde = new Order
                {
                    CustomerID = order1.CustomerID,
                    OrderDate = order1.OrderDate,
                    TotalAmount = order1.TotalAmount,
                    PaymentStatus = order1.PaymentStatus, // Payment status is mapped correctly
                    PaymentMethod = order1.PaymentMethod, // Custom payment method from Order1
                    ShippingMethod = order1.ShippingMethod, // Custom shipping method from Order1
                    ShippingAddress = order1.ShippingAddress, // Custom shipping address from Order1
                                                              // OrderDetails mapping
                    OrderDetail = order1.OrderDetail.Select(item => new OrderDetail
                    {
                        Quantity = item.Quantity,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                try
                {
                    // Save the order to the database
                    db.Order.Add(orde);
                    db.SaveChanges();

                    // Clear cart after successful order
                    Session["Cart"] = null;

                    // Redirect to order success page
                    return RedirectToAction("OrderSuccess", new { id = orde.OrderID });
                }
                catch (Exception ex)
                {
                    // Log error and inform user
                    ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình xử lý đơn hàng. Vui lòng thử lại.");
                }
            }

            return View(model);
        }
    }
}