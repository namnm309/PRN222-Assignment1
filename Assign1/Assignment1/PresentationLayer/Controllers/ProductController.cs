using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;

public class ProductsController : Controller
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProductViewModel vm)
    {
        var (ok, err, list) = await _service.SearchAsync(vm.Q, vm.BrandId, vm.MinPrice, vm.MaxPrice, vm.InStock, vm.IsActive);
        if (!ok) { ModelState.AddModelError("", err); }
        return View(list); 
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var (ok, err, product) = await _service.GetAsync(id);
        if (!ok) return NotFound();
        return View(product); 
    }
}
