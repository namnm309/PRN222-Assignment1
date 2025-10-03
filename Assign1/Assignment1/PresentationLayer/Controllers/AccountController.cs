using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly IAuthenService _authenService;

        public AccountController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authenService.LoginAsync(model.Email, model.Password);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(model);
            }

            
            HttpContext.Session.SetString("UserId", result.User.Id.ToString());
            HttpContext.Session.SetString("UserFullName", result.User.FullName);
            HttpContext.Session.SetString("UserEmail", result.User.Email);
            HttpContext.Session.SetString("UserRole", result.User.Role.ToString());
            
            Console.WriteLine($"[DEBUG] User logged in: {result.User.Email}, Role: {result.User.Role}, DealerId: {result.User.DealerId}");
 
            if (result.User.Role == DataAccessLayer.Enum.UserRole.DealerManager || 
                result.User.Role == DataAccessLayer.Enum.UserRole.DealerStaff)
            {
                if (result.User.DealerId.HasValue)
                {
                    HttpContext.Session.SetString("DealerId", result.User.DealerId.Value.ToString());
                    Console.WriteLine($"[DEBUG] DealerId saved to session: {result.User.DealerId.Value}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] WARNING: Dealer user has no DealerId assigned!");
                }
            }
            
            TempData["LoginMessage"] = "Đăng nhập thành công";
            
            
            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserFullName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("DealerId");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
