using BusinessLayer.DTO;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin,EVMStaff")]
    public class DealerController : Controller
    {
        private readonly IDealerService _service;

        public DealerController(IDealerService service)
        {
            _service = service;
        }

        // GET: Dealer
        public async Task<IActionResult> Index()
        {
            var dealers = await _service.GetAllAsync();
            return View(dealers); // => Views/Dealer/Index.cshtml
        }

        // GET: Dealer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dealer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDealerDto dto)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Dealer/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();
            return View(dealer);
        }

        // POST: Dealer/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DealerDto dto)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Dealer/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();
            return View(dealer);
        }

        // POST: Dealer/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // chỉ Admin mới xóa được
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
