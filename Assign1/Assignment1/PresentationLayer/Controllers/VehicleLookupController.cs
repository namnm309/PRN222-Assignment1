using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class VehicleLookupController : BaseDashboardController
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _dbContext;

        public VehicleLookupController(IProductService productService, AppDbContext dbContext)
        {
            _productService = productService;
            _dbContext = dbContext;
        }

        // GET: VehicleLookup/Index - Tra cứu xe cho DealerStaff
        [HttpGet]
        public async Task<IActionResult> Index(string search = "", Guid? brandId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            // Lấy DealerId từ session nếu là DealerStaff/DealerManager
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (!string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out var dealerId))
            {
                ViewBag.DealerId = dealerId;
            }

            var query = _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Filter theo search term
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Sku.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Brand.Name.ToLower().Contains(searchTerm));
            }

            // Filter theo brand
            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // Filter theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await query
                .OrderBy(p => p.Name)
                .Select(p => new VehicleLookupViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand.Name,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            // Load brands cho filter dropdown
            try
            {
                var brands = await _dbContext.Brand
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new { b.Id, b.Name })
                    .ToListAsync();
                
                // Convert to List<dynamic> để tránh cast error
                ViewBag.Brands = brands.Select(b => new { b.Id, b.Name }).Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi load brands, set empty list
                ViewBag.Brands = new List<dynamic>();
                Console.WriteLine($"Error loading brands: {ex.Message}");
            }

            ViewBag.Search = search;
            ViewBag.SelectedBrandId = brandId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products);
        }

        // GET: VehicleLookup/Detail/id - Chi tiết xe
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var product = await _dbContext.Product
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }

            var viewModel = new VehicleLookupViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description,
                Price = product.Price,
                BrandName = product.Brand.Name,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return View(viewModel);
        }
    }
}
