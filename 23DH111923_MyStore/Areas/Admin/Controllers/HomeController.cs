using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using _23DH111923_MyStore.Models;
using _23DH111923_MyStore.Models.ViewModel;
using PagedList;

namespace _23DH111923_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/Home
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Product.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                           p.ProductDecription.Contains(searchTerm) ||
                           p.Category.CategoryName.Contains(searchTerm));
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(10).ToList();

            model.NewProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(20).ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            // Kiểm tra xem id sản phẩm có hợp lệ không
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm sản phẩm theo id
            Product pro = db.Product.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // Lấy tất cả các sản phẩm cùng danh mục
            var products = db.Product.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            // Tạo một đối tượng ProductDetailsVM để chứa thông tin sản phẩm và các sản phẩm liên quan
            ProductDetailsVM model = new ProductDetailsVM();

            // Xử lý phân trang
            int pageNumber = page ?? 1; // Lấy số trang hiện tại (mặc định là 1 nếu không có giá trị)
            int pageSize = model.PageSize; // Số sản phẩm mỗi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize).ToList();
            model.TopProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            // Cập nhật số lượng sản phẩm nếu có
            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            return View(model);
        }
            public ActionResult Login()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Login(LoginVM model)
            {
                if (ModelState.IsValid)
                {
                    var user = db.User.SingleOrDefault(u => u.Username == model.Username
                                                           && u.Password == model.Password
                                                           && u.UserRole == "Customer");

                    if (user != null)
                    {
                        // Lưu trạng thái đăng nhập vào session
                        Session["Username"] = user.Username;
                        Session["UserRole"] = user.UserRole;

                        // Lưu thông tin xác thực người dùng vào cookie
                        FormsAuthentication.SetAuthCookie(user.Username, false);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                    }
                }

                return View(model);
            }
        
    }
}