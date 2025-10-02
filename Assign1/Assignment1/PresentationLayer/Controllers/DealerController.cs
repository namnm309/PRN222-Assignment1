using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

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

        public async Task<IActionResult> Index()
        {
            var dealers = await _service.GetAllAsync();
            var vm = dealers.Select(d => new DealerViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Address = d.Address,
                Phone = d.phone,
                IsActive = d.IsActive
            });
            return View(vm);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DealerViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dealer = new Dealer
            {
                Id = Guid.NewGuid(),
                Name = vm.Name,
                Address = vm.Address,
                phone = vm.Phone,
                IsActive = vm.IsActive
            };

            await _service.CreateAsync(dealer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();

            var vm = new DealerViewModel
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Address = dealer.Address,
                Phone = dealer.phone,
                IsActive = dealer.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DealerViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();

            dealer.Name = vm.Name;
            dealer.Address = vm.Address;
            dealer.phone = vm.Phone;
            dealer.IsActive = vm.IsActive;

            await _service.UpdateAsync(id, dealer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var dealer = await _service.GetByIdAsync(id);
            if (dealer == null) return NotFound();

            var vm = new DealerViewModel
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Address = dealer.Address,
                Phone = dealer.phone,
                IsActive = dealer.IsActive
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
