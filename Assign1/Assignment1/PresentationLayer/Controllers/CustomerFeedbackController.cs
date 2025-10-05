using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using DataAccessLayer.Enum;

namespace PresentationLayer.Controllers
{
    public class CustomerFeedbackController : BaseDashboardController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;

        public CustomerFeedbackController(
            IFeedbackService feedbackService,
            ICustomerService customerService,
            IProductService productService,
            IEVMReportService evmService,
            IMappingService mappingService)
        {
            _feedbackService = feedbackService;
            _customerService = customerService;
            _productService = productService;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        // GET: CustomerFeedback - Danh sách phản hồi khách hàng
        [HttpGet]
        public async Task<IActionResult> Index(string? status = null, Guid? productId = null, string? search = null)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được truy cập
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy danh sách sản phẩm cho filter
            var (productsOk, _, products) = await _productService.SearchAsync(null, null, null, null, null, true);
            ViewBag.Products = productsOk ? products : new List<DataAccessLayer.Entities.Product>();

            // Lấy danh sách phản hồi
            var (success, error, feedbacks) = await _feedbackService.GetAllAsync();

            if (!success)
            {
                TempData["Error"] = error;
                return View(new List<FeedbackViewModel>());
            }

            // Filter theo điều kiện
            var filteredFeedbacks = feedbacks.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                // Có thể filter theo trạng thái xử lý (nếu có field này)
                // filteredFeedbacks = filteredFeedbacks.Where(f => f.Status == status);
            }

            if (productId.HasValue)
            {
                filteredFeedbacks = filteredFeedbacks.Where(f => f.ProductId == productId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchTerm = search.ToLower();
                filteredFeedbacks = filteredFeedbacks.Where(f => 
                    f.Comment.ToLower().Contains(searchTerm) ||
                    (f.Customer != null && f.Customer.FullName.ToLower().Contains(searchTerm))
                );
            }

            // Map entities to view models
            var feedbackViewModels = _mappingService.MapToFeedbackViewModels(filteredFeedbacks.ToList());

            // Truyền filter values để giữ lại giá trị
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedProductId = productId;
            ViewBag.SearchTerm = search;

            return View(feedbackViewModels);
        }

        // GET: CustomerFeedback/Detail/{id} - Chi tiết phản hồi
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, feedback) = await _feedbackService.GetByIdAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entity to view model
            var feedbackViewModel = _mappingService.MapToFeedbackViewModel(feedback);
            
            return View(feedbackViewModel);
        }

        // GET: CustomerFeedback/Reply/{id} - Trả lời phản hồi
        [HttpGet]
        public async Task<IActionResult> Reply(Guid id)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, feedback) = await _feedbackService.GetByIdAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entity to view model
            var feedbackViewModel = _mappingService.MapToFeedbackViewModel(feedback);
            
            return View(feedbackViewModel);
        }

        // POST: CustomerFeedback/Reply/{id} - Xử lý trả lời phản hồi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string replyMessage)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrWhiteSpace(replyMessage))
            {
                TempData["Error"] = "Vui lòng nhập nội dung trả lời.";
                return RedirectToAction(nameof(Reply), new { id });
            }

            // Lấy thông tin user hiện tại
            var currentUserId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out Guid userId))
            {
                TempData["Error"] = "Không xác định được người dùng.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Cập nhật phản hồi với trả lời
            var (success, error) = await _feedbackService.ReplyToFeedbackAsync(id, replyMessage, userId);
            
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Reply), new { id });
            }

            TempData["Success"] = "Trả lời phản hồi thành công!";
            return RedirectToAction(nameof(Detail), new { id });
        }

        // GET: CustomerFeedback/Statistics - Thống kê phản hồi
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, feedbacks) = await _feedbackService.GetAllAsync();
            if (!success)
            {
                TempData["Error"] = error;
                return View(new { });
            }

            // Tính toán thống kê
            var totalFeedbacks = feedbacks.Count;
            var averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
            var ratingDistribution = feedbacks.GroupBy(f => f.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .OrderBy(x => x.Rating)
                .ToList();

            var recentFeedbacks = feedbacks.OrderByDescending(f => f.CreatedAt).Take(5).ToList();

            var statistics = new
            {
                TotalFeedbacks = totalFeedbacks,
                AverageRating = Math.Round(averageRating, 2),
                RatingDistribution = ratingDistribution,
                RecentFeedbacks = recentFeedbacks
            };

            return View(statistics);
        }

        // GET: CustomerFeedback/Export - Xuất báo cáo phản hồi
        [HttpGet]
        public async Task<IActionResult> Export(string? format = "excel")
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (success, error, feedbacks) = await _feedbackService.GetAllAsync();
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            // Map entities to view models
            var feedbackViewModels = _mappingService.MapToFeedbackViewModels(feedbacks);

            if (format?.ToLower() == "excel")
            {
                // Tạo Excel file (cần implement Excel export logic)
                // return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "customer_feedback.xlsx");
                TempData["Info"] = "Chức năng xuất Excel sẽ được triển khai trong phiên bản tiếp theo.";
                return RedirectToAction(nameof(Index));
            }

            // Xuất CSV
            var csv = GenerateCsv(feedbackViewModels);
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(csvBytes, "text/csv", "customer_feedback.csv");
        }

        private string GenerateCsv(List<FeedbackViewModel> feedbacks)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("ID,Khách hàng,Sản phẩm,Đánh giá,Nội dung,Ngày tạo");

            foreach (var feedback in feedbacks)
            {
                csv.AppendLine($"{feedback.Id},{feedback.CustomerName},{feedback.ProductName},{feedback.Rating},\"{feedback.Comment}\",{feedback.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            return csv.ToString();
        }
    }
}
