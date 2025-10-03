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
            
            return View();
        }

        public IActionResult Products(string? q = null, Guid? brandId = null)
        {
            
            return RedirectToAction("Index", "ProductManagement", new { q, brandId });
        }

        public IActionResult Orders()
        {
            
            return View();
        }

        public IActionResult Reports()
        {
            
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
