using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class CustomerFeedbackController : BaseDashboardController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly AppDbContext _dbContext;

        public CustomerFeedbackController(IFeedbackService feedbackService, AppDbContext dbContext)
        {
            _feedbackService = feedbackService;
            _dbContext = dbContext;
        }

        // GET: CustomerFeedback/Index - Danh sách phản hồi cho DealerStaff
        [HttpGet]
        public async Task<IActionResult> Index(string search = "", Guid? productId = null, int? rating = null)
        {
            // Lấy DealerId từ session nếu là DealerStaff/DealerManager
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (!string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out var dealerId))
            {
                ViewBag.DealerId = dealerId;
            }

            var query = _dbContext.Feedback
                .Include(f => f.Customer)
                .Include(f => f.Product)
                .ThenInclude(p => p.Brand)
                .AsQueryable();

            // Filter theo search term (tên khách hàng, email, nội dung phản hồi)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLower();
                query = query.Where(f =>
                    f.Customer.FullName.ToLower().Contains(searchTerm) ||
                    f.Customer.Email.ToLower().Contains(searchTerm) ||
                    f.Comment.ToLower().Contains(searchTerm) ||
                    f.Product.Name.ToLower().Contains(searchTerm));
            }

            // Filter theo sản phẩm
            if (productId.HasValue)
            {
                query = query.Where(f => f.ProductId == productId.Value);
            }

            // Filter theo rating
            if (rating.HasValue)
            {
                query = query.Where(f => f.Rating == rating.Value);
            }

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new CustomerFeedbackViewModel
                {
                    Id = f.Id,
                    CustomerName = f.Customer.FullName,
                    CustomerEmail = f.Customer.Email,
                    CustomerPhone = f.Customer.PhoneNumber,
                    ProductName = f.Product.Name,
                    ProductSku = f.Product.Sku,
                    BrandName = f.Product.Brand.Name,
                    Comment = f.Comment,
                    Rating = f.Rating,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();

            // Load products cho filter dropdown
            try
            {
                var products = await _dbContext.Product
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .Select(p => new { p.Id, p.Name })
                    .ToListAsync();
                
                // Convert to List<dynamic> để tránh cast error
                ViewBag.Products = products.Select(p => new { p.Id, p.Name }).Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi load products, set empty list
                ViewBag.Products = new List<dynamic>();
                Console.WriteLine($"Error loading products: {ex.Message}");
            }

            ViewBag.Search = search;
            ViewBag.SelectedProductId = productId;
            ViewBag.SelectedRating = rating;

            return View(feedbacks);
        }

        // GET: CustomerFeedback/Detail/id - Chi tiết phản hồi
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var feedback = await _dbContext.Feedback
                .Include(f => f.Customer)
                .Include(f => f.Product)
                .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
            {
                TempData["Error"] = "Không tìm thấy phản hồi";
                return RedirectToAction("Index");
            }

            var viewModel = new CustomerFeedbackDetailViewModel
            {
                Id = feedback.Id,
                CustomerName = feedback.Customer.FullName,
                CustomerEmail = feedback.Customer.Email,
                CustomerPhone = feedback.Customer.PhoneNumber,
                CustomerAddress = feedback.Customer.Address,
                ProductName = feedback.Product.Name,
                ProductSku = feedback.Product.Sku,
                BrandName = feedback.Product.Brand.Name,
                Comment = feedback.Comment,
                Rating = feedback.Rating,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt
            };

            return View(viewModel);
        }

        // POST: CustomerFeedback/Delete/id - Xóa phản hồi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (ok, err) = await _feedbackService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Msg"] = "Đã xóa phản hồi thành công";
            }
            
            return RedirectToAction("Index");
        }
    }
}
