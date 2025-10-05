using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using DataAccessLayer.Enum;

namespace PresentationLayer.Controllers
{
    public class FeedbackController : BaseDashboardController
    {
        private readonly IFeedbackService _service;
        private readonly IMappingService _mappingService;
        private readonly IProductService _productService;
        
        public FeedbackController(IFeedbackService service, IMappingService mappingService, IProductService productService)
        {
            _service = service;
            _mappingService = mappingService;
            _productService = productService;
        }

        // GET: Feedback - Trang xem tất cả feedback cho Dealer Staff
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = null, int? rating = null, Guid? productId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Debug: Log thông tin user
            System.Diagnostics.Debug.WriteLine($"Feedback Index - UserRole: {CurrentUserRole}, IsDealer: {IsDealer()}");
            Console.WriteLine($"Feedback Index - UserRole: {CurrentUserRole}, IsDealer: {IsDealer()}");
            
            // Chỉ Dealer Staff và Dealer Manager mới được truy cập
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            try
            {
                var (success, error, feedbacks) = await _service.GetAllAsync();
                if (!success)
                {
                    TempData["Error"] = error;
                    return View(new List<FeedbackViewModel>());
                }

                // Map entities to view models
                var feedbackViewModels = _mappingService.MapToFeedbackViewModels(feedbacks);

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    feedbackViewModels = feedbackViewModels.Where(f => 
                        f.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        f.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (f.Comment != null && f.Comment.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (rating.HasValue)
                {
                    feedbackViewModels = feedbackViewModels.Where(f => f.Rating == rating.Value).ToList();
                }

                if (productId.HasValue)
                {
                    feedbackViewModels = feedbackViewModels.Where(f => f.ProductId == productId.Value).ToList();
                }

                if (fromDate.HasValue)
                {
                    feedbackViewModels = feedbackViewModels.Where(f => f.CreatedAt.Date >= fromDate.Value.Date).ToList();
                }

                if (toDate.HasValue)
                {
                    feedbackViewModels = feedbackViewModels.Where(f => f.CreatedAt.Date <= toDate.Value.Date).ToList();
                }

                // Load products for filter dropdown
                var (productSuccess, productError, products) = await _productService.SearchAsync(null, null, null, null, null, true);
                if (productSuccess)
                {
                    ViewBag.Products = products;
                }

                // Set ViewBag for filter dropdowns
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedRating = rating;
                ViewBag.SelectedProductId = productId;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                return View(feedbackViewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return View(new List<FeedbackViewModel>());
            }
        }

        // GET: Feedback/Test - Test action để debug
        [HttpGet]
        public IActionResult Test()
        {
            return Json(new { 
                message = "Feedback controller is working!",
                userRole = CurrentUserRole?.ToString(),
                isDealer = IsDealer(),
                userName = CurrentUserName,
                sessionRole = HttpContext.Session.GetString("UserRole"),
                sessionName = HttpContext.Session.GetString("UserFullName")
            });
        }

        // GET: Feedback/Simple - Simple test page
        [HttpGet]
        public IActionResult Simple()
        {
            var debugInfo = $@"
                <h1>Feedback Simple Test</h1>
                <p><strong>User:</strong> {CurrentUserName}</p>
                <p><strong>Role:</strong> {CurrentUserRole}</p>
                <p><strong>Is Dealer:</strong> {IsDealer()}</p>
                <p><strong>Session Role:</strong> {HttpContext.Session.GetString("UserRole")}</p>
                <p><strong>Session Name:</strong> {HttpContext.Session.GetString("UserFullName")}</p>
                <hr>
                <a href='/Feedback'>Go to Feedback Index</a>
            ";
            return Content(debugInfo, "text/html");
        }

        // GET: Feedback/Debug - Debug page
        [HttpGet]
        public IActionResult Debug()
        {
            var debugInfo = $@"
                <h1>Feedback Controller Debug</h1>
                <p><strong>User Role:</strong> {CurrentUserRole}</p>
                <p><strong>User Name:</strong> {CurrentUserName}</p>
                <p><strong>User Email:</strong> {CurrentUserEmail}</p>
                <p><strong>Is Dealer:</strong> {IsDealer()}</p>
                <p><strong>Session UserRole:</strong> {HttpContext.Session.GetString("UserRole")}</p>
                <p><strong>Session UserFullName:</strong> {HttpContext.Session.GetString("UserFullName")}</p>
                <p><strong>Session UserEmail:</strong> {HttpContext.Session.GetString("UserEmail")}</p>
                <hr>
                <a href='/Feedback'>Go to Feedback Index</a>
            ";
            return Content(debugInfo, "text/html");
        }

        // GET: Feedback/Public - Public test page (no authentication required)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Public()
        {
            return Content("Feedback Controller is working! Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "text/plain");
        }

        // GET: Feedback/CheckAccess - Check access for Dealer Staff
        [HttpGet]
        public IActionResult CheckAccess()
        {
            var result = new
            {
                IsAuthenticated = !string.IsNullOrEmpty(CurrentUserName),
                UserRole = CurrentUserRole?.ToString(),
                IsDealer = IsDealer(),
                SessionRole = HttpContext.Session.GetString("UserRole"),
                SessionName = HttpContext.Session.GetString("UserFullName"),
                CanAccessFeedback = IsDealer(),
                Message = IsDealer() ? "Bạn có quyền truy cập Feedback" : "Bạn không có quyền truy cập Feedback"
            };
            
            return Json(result);
        }

        // GET: Feedback/Test2 - Another test
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Test2()
        {
            return Ok("Test2 is working!");
        }

        [HttpGet]
        public async Task<IActionResult> ByProduct(Guid productId)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được truy cập
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, list) = await _service.GetByProductAsync(productId);
            if (!ok) { ModelState.AddModelError("", err); }
            ViewBag.ProductId = productId;
            return View(list); 
        }

        [HttpGet]
        public IActionResult Create(Guid productId, Guid customerId)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được tạo feedback
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var viewModel = new FeedbackViewModel 
            { 
                ProductId = productId, 
                CustomerId = customerId 
            };
            
            // Set ViewBag để load dropdown data
            ViewBag.ProductId = productId;
            ViewBag.CustomerId = customerId;
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackViewModel vm)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được tạo feedback
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền thực hiện thao tác này.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid) return View(vm);
            var (ok, err, _) = await _service.CreateAsync(vm.CustomerId, vm.ProductId, vm.Comment, vm.Rating);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                return View(vm);
            }
            TempData["Msg"] = "Cảm ơn phản hồi của bạn.";
            return RedirectToAction(nameof(ByProduct), new { productId = vm.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid productId)
        {
            // Chỉ Dealer Staff và Dealer Manager mới được xóa feedback
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền thực hiện thao tác này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _service.DeleteAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã xóa phản hồi.";
            return RedirectToAction(nameof(ByProduct), new { productId });
        }
    }
}
