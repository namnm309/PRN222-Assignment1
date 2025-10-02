using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin,EVMStaff")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetAllAsync();
            var vm = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                ModelName = c.ModelName,
                Color = c.color,
                Variant = c.varian,
                IsActive = c.IsActive
            });
            return View(vm);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                ModelName = vm.ModelName,
                color = vm.Color,
                varian = vm.Variant,
                IsActive = vm.IsActive
            };

            await _service.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryViewModel
            {
                Id = category.Id,
                ModelName = category.ModelName,
                Color = category.color,
                Variant = category.varian,
                IsActive = category.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.ModelName = vm.ModelName;
            category.color = vm.Color;
            category.varian = vm.Variant;
            category.IsActive = vm.IsActive;

            await _service.UpdateAsync(id, category);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryViewModel
            {
                Id = category.Id,
                ModelName = category.ModelName,
                Color = category.color,
                Variant = category.varian,
                IsActive = category.IsActive
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
