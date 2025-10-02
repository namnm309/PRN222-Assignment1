using BusinessLayer.DTO;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        // GET: Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetAllAsync();

            // map DTO -> ViewModel
            var viewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                ModelName = c.ModelName,
                Color = c.Color,
                Variant = c.Varian,
                IsActive = c.IsActive
            }).ToList();

            return View(viewModels);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreateCategoryDto
            {
                ModelName = model.ModelName,
                Color = model.Color,
                Varian = model.Variant
            };

            await _service.CreateAsync(dto);
            TempData["Message"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CategoryViewModel
            {
                Id = dto.Id,
                ModelName = dto.ModelName,
                Color = dto.Color,
                Variant = dto.Varian,
                IsActive = dto.IsActive
            };

            return View(vm);
        }

        // POST: Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CategoryDto
            {
                Id = model.Id,
                ModelName = model.ModelName,
                Color = model.Color,
                Varian = model.Variant,
                IsActive = model.IsActive
            };

            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            TempData["Message"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CategoryViewModel
            {
                Id = dto.Id,
                ModelName = dto.ModelName,
                Color = dto.Color,
                Variant = dto.Varian,
                IsActive = dto.IsActive
            };

            return View(vm);
        }

        // POST: Category/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);
            TempData["Message"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
