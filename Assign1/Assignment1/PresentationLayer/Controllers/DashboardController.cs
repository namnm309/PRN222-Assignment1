using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;

namespace PresentationLayer.Controllers
{
    public class DashboardController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;

        public DashboardController(IProductService productService, IBrandService brandService)
        {
            _productService = productService;
            _brandService = brandService;
        }

        public IActionResult Index()
        {
            // ViewBag đã được set tự động từ BaseDashboardController
            return View();
        }

        public async Task<IActionResult> Products(string? q = null, Guid? brandId = null)
        {
            // Load dữ liệu sản phẩm thực sự
            var (ok, err, products) = await _productService.SearchAsync(q, brandId, null, null, null, true); // Chỉ hiển thị sản phẩm active
            if (!ok)
            {
                TempData["Error"] = err;
            }

            // Load brands để filter
            var (brandOk, brandErr, brands) = await _brandService.GetAllAsync();
            ViewBag.Brands = brandOk ? brands : new List<DataAccessLayer.Entities.Brand>();
            ViewBag.SearchQuery = q;
            ViewBag.SelectedBrandId = brandId;

            return View(products ?? new List<DataAccessLayer.Entities.Product>());
        }

        public IActionResult Orders()
        {
            // Ví dụ trang Orders
            return View();
        }

        public IActionResult Reports()
        {
            // Ví dụ trang Reports
            return View();
        }

        public IActionResult EVMDashboard()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập EVM Dashboard.";
                return RedirectToAction("Index", "Dashboard");
            }
            
            return RedirectToAction("Index", "EVMDashboard");
        }
    }
}
