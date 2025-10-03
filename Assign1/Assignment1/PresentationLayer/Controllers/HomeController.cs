using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term));
            }

            var products = await query
                .OrderBy(p => p.Name)
                .Take(5)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand.Name,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> All(string search)
        {
            var query = _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term));
            }

            var products = await query
                .OrderBy(p => p.Name)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand.Name,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> TestDrive()
        {
            var products = await _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new HomeProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price,
                    BrandName = p.Brand.Name,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();


            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
