using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;
using PresentationLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class UserManagementController : BaseDashboardController
    {
        private readonly IAuthenService _authenService;
        private readonly IEVMReportService _evmService;

        public UserManagementController(IAuthenService authenService, IEVMReportService evmService)
        {
            _authenService = authenService;
            _evmService = evmService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            List<Users> users;

            if (userRole == "Admin")
            {
                users = await _evmService.GetAllUsersAsync();
            }
            else if (userRole == "DealerManager" && !string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                users = await _evmService.GetUsersByDealerAsync(dealerId);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDealerManager()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin mới có quyền tạo Dealer Manager.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDealerManager(UserCreateViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin mới có quyền tạo Dealer Manager.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            
            var (existingUser, _) = await _authenService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            var (success, error, user) = await _authenService.RegisterAsync(
                model.FullName, 
                model.Email, 
                model.Password, 
                model.PhoneNumber, 
                model.Address, 
                UserRole.DealerManager,
                model.DealerId
            );

            if (!success)
            {
                TempData["Error"] = error;
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            TempData["Success"] = $"Tạo tài khoản Dealer Manager thành công! Email: {model.Email}";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public IActionResult CreateDealerStaff()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userRole != "DealerManager")
            {
                TempData["Error"] = "Chỉ Dealer Manager mới có quyền tạo Dealer Staff.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDealerStaff(UserCreateViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (userRole != "DealerManager")
            {
                TempData["Error"] = "Chỉ Dealer Manager mới có quyền tạo Dealer Staff.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Không xác định được dealer của bạn.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            var (existingUser, _) = await _authenService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                return View(model);
            }

            
            var (success, error, user) = await _authenService.RegisterAsync(
                model.FullName, 
                model.Email, 
                model.Password, 
                model.PhoneNumber, 
                model.Address, 
                UserRole.DealerStaff,
                dealerId 
            );

            if (!success)
            {
                TempData["Error"] = error;
                return View(model);
            }

            TempData["Success"] = $"Tạo tài khoản Dealer Staff thành công! Email: {model.Email}";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            var user = await _evmService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            
            if (userRole != "Admin")
            {
                
                if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    if (user.DealerId != dealerId || user.Role != UserRole.DealerStaff)
                    {
                        TempData["Error"] = "Bạn chỉ có thể chỉnh sửa nhân viên của chính đại lý mình.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa người dùng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive
            };

            if (IsAdmin())
            {
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (!ModelState.IsValid)
            {
                if (IsAdmin())
                {
                    ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                }
                return View(model);
            }

            var user = await _evmService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            
            if (userRole != "Admin")
            {
                
                if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                    if (user.DealerId != dealerId || user.Role != UserRole.DealerStaff)
                    {
                        TempData["Error"] = "Bạn chỉ có thể chỉnh sửa nhân viên của chính đại lý mình.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa người dùng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var success = await _evmService.UpdateUserAsync(user);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật thông tin người dùng.";
                return View(model);
            }

            TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            var user = await _evmService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            
            if (userRole == "Admin")
            {
                var currentUserEmail = HttpContext.Session.GetString("UserEmail");
                if (user.Email == currentUserEmail)
                {
                    TempData["Error"] = "Bạn không thể xóa tài khoản của chính mình.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            else if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                if (user.DealerId != dealerId || user.Role != UserRole.DealerStaff)
                {
                    TempData["Error"] = "Bạn chỉ có thể xóa nhân viên của chính đại lý mình.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền xóa người dùng này.";
                return RedirectToAction(nameof(Index));
            }

            
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var success = await _evmService.UpdateUserAsync(user);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa người dùng.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Xóa người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}

