using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Services;
using PresentationLayer.Models;
using System.Linq;

namespace PresentationLayer.Controllers
{
    public class TestDriveController : BaseDashboardController
    {
        private readonly ITestDriveService _service;
        private readonly IEVMReportService _evmService;
        
        public TestDriveController(ITestDriveService service, IEVMReportService evmService)
        {
            _service = service;
            _evmService = evmService;
        }

        // Danh sách lịch hẹn cho Dealer Staff/Manager
        [HttpGet]
        public async Task<IActionResult> Index(Guid? dealerId = null, string? status = null)
        {
            ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            ViewBag.SelectedDealerId = dealerId;
            ViewBag.SelectedStatus = status;

            // Nếu là DealerManager hoặc DealerStaff, chỉ hiển thị test drive của đại lý mình
            if (ViewBag.UserRole == DataAccessLayer.Enum.UserRole.DealerManager || 
                ViewBag.UserRole == DataAccessLayer.Enum.UserRole.DealerStaff)
            {
                dealerId = ViewBag.DealerId;
            }

            var (ok, err, testDrives) = await _service.GetAllAsync(dealerId, status);
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<DataAccessLayer.Entities.TestDrive>());
            }

            return View(testDrives);
        }

        // Lịch hẹn của Customer
        [HttpGet]
        public async Task<IActionResult> MyTestDrives(Guid customerId)
        {
            var (ok, err, testDrives) = await _service.GetByCustomerAsync(customerId);
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<DataAccessLayer.Entities.TestDrive>());
            }
            return View(testDrives);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Guid productId, Guid? dealerId = null)
        {
            // Nếu không có dealerId, tự động chọn dealer đầu tiên
            if (!dealerId.HasValue)
            {
                var dealers = await _evmService.GetAllDealersAsync();
                if (dealers.Any())
                {
                    dealerId = dealers.First().Id;
                }
                else
                {
                    TempData["Error"] = "Hiện tại chưa có đại lý nào có sẵn. Vui lòng liên hệ trực tiếp.";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(new TestDriveViewModel { ProductId = productId, DealerId = dealerId.Value });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestDriveViewModel vm)
        {
            if (vm.ScheduledDate == default)
            {
                // Nếu view không truyền lên (ẩn), đặt mặc định sau 2 giờ kể từ hiện tại (UTC)
                vm.ScheduledDate = DateTime.UtcNow.AddHours(2);
            }

            var (ok, err, td) = await _service.CreatePublicAsync(
                vm.CustomerName, 
                vm.CustomerPhone, 
                vm.CustomerEmail, 
                vm.Notes,
                vm.ProductId, 
                vm.DealerId, 
                vm.ScheduledDate
            );
            
            if (!ok) 
            { 
                ModelState.AddModelError("", err); 
                return View(vm); 
            }

            TempData["Msg"] = "Đăng ký lái thử thành công! Mã đặt lịch: " + td.Id.ToString().Substring(0, 8).ToUpper();
            TempData["Success"] = "true";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, td) = await _service.GetAsync(id);
            if (!ok) return NotFound();
            return View(td);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var (ok, err, td) = await _service.ConfirmAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã xác nhận lịch hẹn.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid id, bool success = true)
        {
            var (ok, err, td) = await _service.CompleteAsync(id, success);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = success ? "Lái thử thành công" : "Lái thử thất bại";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var (ok, err, td) = await _service.CancelAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã hủy lịch hẹn.";
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
