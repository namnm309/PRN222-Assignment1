using BusinessLayer.DTO;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
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

            var viewModels = dealers.Select(d => new DealerViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Address = d.Address,
                Phone = d.Phone,
                IsActive = d.IsActive
            }).ToList();

            return View(viewModels); // => Views/Dealer/Index.cshtml
        }

        // GET: Dealer/Create
        public IActionResult Create()
        {
            return View(new DealerViewModel());
        }

        // POST: Dealer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DealerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreateDealerDto
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone
            };

            await _service.CreateAsync(dto);
            TempData["Message"] = "Dealer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Dealer/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();

            var vm = new DealerViewModel
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Address = dealer.Address,
                Phone = dealer.Phone,
                IsActive = dealer.IsActive
            };

            return View(vm);
        }

        // POST: Dealer/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DealerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new DealerDto
            {
                Id = model.Id,
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                IsActive = model.IsActive
            };

            await _service.UpdateAsync(id, dto);
            TempData["Message"] = "Dealer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Dealer/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();

            var vm = new DealerViewModel
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Address = dealer.Address,
                Phone = dealer.Phone,
                IsActive = dealer.IsActive
            };

            return View(vm);
        }

        // POST: Dealer/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // chỉ Admin mới xóa được
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            TempData["Message"] = "Dealer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
