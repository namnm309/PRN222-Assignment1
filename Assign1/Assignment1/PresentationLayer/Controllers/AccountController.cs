using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        //gọi từ BusinessLayer để sử dụng 
        private readonly IAuthenService _authenService;

        //constructor
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

            // Lưu thông tin user vào session
            HttpContext.Session.SetString("UserFullName", result.User.FullName);
            HttpContext.Session.SetString("UserEmail", result.User.Email);
            HttpContext.Session.SetString("UserRole", result.User.Role.ToString());
            
            TempData["LoginMessage"] = "Đăng nhập thành công";
            
            // Redirect vào Dashboard thay vì Homepage
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authenService.RegisterAsync(model.FullName, model.Email, model.Password);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(model);
            }

            TempData["RegisterMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserFullName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
