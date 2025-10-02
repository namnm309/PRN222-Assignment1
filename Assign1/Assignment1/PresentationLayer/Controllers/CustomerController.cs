using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;
using DataAccessLayer.Entities;

public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Profile(Guid id)
    {
        var (ok, err, c) = await _service.GetAsync(id);
        if (!ok) return NotFound();
        var vm = new CustomerViewModel
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            IsActive = c.IsActive
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(CustomerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var entity = new Customer
        {
            Id = vm.Id,
            FullName = vm.FullName,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber,
            Address = vm.Address,
            IsActive = vm.IsActive,
            UpdatedAt = DateTime.UtcNow
        };
        var (ok, err, _) = await _service.UpdateProfileAsync(entity);
        if (!ok) { ModelState.AddModelError("", err); return View(vm); }
        TempData["Msg"] = "Cập nhật thành công.";
        return RedirectToAction(nameof(Profile), new { id = vm.Id });
    }
}
