using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using DataAccessLayer.Entities;

namespace PresentationLayer.Controllers
{
    public class ProductManagementController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly IEVMReportService _evmService;
        private readonly IBrandService _brandService;

        public ProductManagementController(IProductService productService, IEVMReportService evmService, IBrandService brandService)
        {
            _productService = productService;
            _evmService = evmService;
            _brandService = brandService;
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            await LoadBrandsToViewBag();
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadBrandsToViewBag();
                return View(model);
            }

            var product = new Product
            {
                Sku = model.Sku,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive,
                BrandId = model.BrandId
            };

            var (ok, err) = await _productService.CreateAsync(product);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                await LoadBrandsToViewBag();
                return View(model);
            }

            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, product) = await _productService.GetAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                BrandId = product.BrandId
            };

            await LoadBrandsToViewBag();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                await LoadBrandsToViewBag();
                return View(model);
            }

            var product = new Product
            {
                Id = model.Id,
                Sku = model.Sku,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive,
                BrandId = model.BrandId
            };

            var (ok, err) = await _productService.UpdateAsync(product);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                await LoadBrandsToViewBag();
                return View(model);
            }

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _productService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadBrandsToViewBag()
        {
            var (ok, err, brands) = await _brandService.GetAllAsync();
            ViewBag.Brands = ok ? brands : new List<Brand>();
        }
    }
}

