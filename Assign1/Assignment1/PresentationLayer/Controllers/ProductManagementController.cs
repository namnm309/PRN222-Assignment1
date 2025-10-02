using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;

namespace PresentationLayer.Controllers
{
    public class ProductManagementController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly IEVMReportService _evmService;

        public ProductManagementController(IProductService productService, IEVMReportService evmService)
        {
            _productService = productService;
            _evmService = evmService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q = null, Guid? brandId = null)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, products) = await _productService.SearchAsync(q, brandId, null, null, null, null);
            if (!ok)
            {
                TempData["Error"] = err;
            }

            ViewBag.Brands = await _evmService.GetAllBrandsAsync();
            ViewBag.SearchQuery = q;
            ViewBag.SelectedBrandId = brandId;

            return View(products ?? new List<DataAccessLayer.Entities.Product>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(Guid id, int stockQuantity)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _productService.UpdateStockAsync(id, stockQuantity);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Cập nhật tồn kho thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

