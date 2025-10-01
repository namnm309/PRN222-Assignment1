using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    public class TestDriveController : Controller
    {
        private readonly ITestDriveService _service;
        public TestDriveController(ITestDriveService service) => _service = service;

        [HttpGet]
        public IActionResult Create(Guid productId, Guid dealerId, Guid customerId)
            => View(new TestDriveViewModel { ProductId = productId, DealerId = dealerId, CustomerId = customerId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestDriveViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (ok, err, td) = await _service.CreateAsync(vm.CustomerId, vm.ProductId, vm.DealerId, vm.ScheduledDate);
            if (!ok) { ModelState.AddModelError("", err); return View(vm); }

            TempData["Msg"] = "Đã tạo lịch hẹn.";
            return RedirectToAction(nameof(Detail), new { id = td.Id });
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
