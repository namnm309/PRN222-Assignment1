using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class DealerController : BaseDashboardController
    {
        private readonly IDealerService _dealerService;
        private readonly IEVMReportService _evmService;

        public DealerController(IDealerService dealerService, IEVMReportService evmService)
        {
            _dealerService = dealerService;
            _evmService = evmService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, dealers) = await _dealerService.GetAllAsync();
            if (!ok)
            {
                TempData["Error"] = err;
                return View(new List<Dealer>());
            }
            return View(dealers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Regions = await _evmService.GetAllRegionsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err, dealer) = await _dealerService.CreateAsync(
                name, phone, address, city, province, regionId, dealerCode,
                contactPerson, email, licenseNumber, creditLimit);

            if (!ok)
            {
                TempData["Error"] = err;
                ViewBag.Regions = await _evmService.GetAllRegionsAsync();
                return View();
            }

            TempData["Success"] = "Tạo đại lý thành công!";
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

            var (ok, err, dealer) = await _dealerService.GetByIdAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Regions = await _evmService.GetAllRegionsAsync();
            return View(dealer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id, string name, string phone, string address, string city, string province,
            Guid? regionId, string dealerCode, string contactPerson, string email,
            string licenseNumber, decimal creditLimit, decimal outstandingDebt, string status, bool isActive)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var (ok, err) = await _dealerService.UpdateAsync(
                id, name, phone, address, city, province, regionId, dealerCode,
                contactPerson, email, licenseNumber, creditLimit, outstandingDebt, status, isActive);

            if (!ok)
            {
                TempData["Error"] = err;
                var (_, __, dealer) = await _dealerService.GetByIdAsync(id);
                ViewBag.Regions = await _evmService.GetAllRegionsAsync();
                return View(dealer);
            }

            TempData["Success"] = "Cập nhật đại lý thành công!";
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

            var (ok, err) = await _dealerService.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = err;
            }
            else
            {
                TempData["Success"] = "Xóa đại lý thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
    //namespace PresentationLayer.Controllers
    //{
    //    [Authorize(Roles = "Admin,EVMStaff")]
    //    public class DealerController : Controller
    //    {
    //        private readonly IDealerService _service;

    //        public DealerController(IDealerService service)
    //        {
    //            _service = service;
    //        }

    //        // GET: Dealer
    //        public async Task<IActionResult> Index()
    //        {
    //            var dealers = await _service.GetAllAsync();
    //            return View(dealers);
    //        }

    //        // GET: Dealer/Create
    //        public IActionResult Create()
    //        {
    //            return View(new DealerViewModel());
    //        }

    //        // POST: Dealer/Create
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<IActionResult> Create(DealerViewModel model)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                await _service.CreateAsync(model);
    //                return RedirectToAction(nameof(Index));
    //            }
    //            return View(model);
    //        }

    //        // GET: Dealer/Edit/{id}
    //        public async Task<IActionResult> Edit(Guid id)
    //        {
    //            var dealer = await _service.GetByIdAsync(id);
    //            if (dealer == null) return NotFound();
    //            return View(dealer);
    //        }

    //        // POST: Dealer/Edit/{id}
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public async Task<IActionResult> Edit(Guid id, DealerViewModel model)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                await _service.UpdateAsync(id, model);
    //                return RedirectToAction(nameof(Index));
    //            }
    //            return View(model);
    //        }

    //        // GET: Dealer/Delete/{id}
    //        public async Task<IActionResult> Delete(Guid id)
    //        {
    //            var dealer = await _service.GetByIdAsync(id);
    //            if (dealer == null) return NotFound();
    //            return View(dealer);
    //        }

    //        // POST: Dealer/Delete/{id}
    //        [HttpPost, ActionName("Delete")]
    //        [ValidateAntiForgeryToken]
    //        [Authorize(Roles = "Admin")] // chỉ Admin mới xóa được
    //        public async Task<IActionResult> DeleteConfirmed(Guid id)
    //        {
    //            await _service.DeleteAsync(id);
    //            return RedirectToAction(nameof(Index));
    //        }
    //    }
    //}
}

