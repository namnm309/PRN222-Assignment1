using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;

namespace PresentationLayer.Controllers
{
    public class CustomerManagementController : BaseDashboardController
    {
        private readonly ICustomerService _customerService;
        private readonly AppDbContext _context;

        public CustomerManagementController(ICustomerService customerService, AppDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdStr = HttpContext.Session.GetString("DealerId");

            List<Customer> customers;

            // Admin và EVM Staff xem tất cả khách hàng
            if (userRole == "Admin" || userRole == "EVMStaff")
            {
                var (ok, err, data) = await _customerService.GetAllAsync();
                if (!ok)
                {
                    TempData["Error"] = err;
                    return View(new List<Customer>());
                }
                customers = data;
            }
            // Dealer chỉ xem khách hàng của mình
            else if ((userRole == "DealerManager" || userRole == "DealerStaff") && !string.IsNullOrEmpty(dealerIdStr))
            {
                var dealerId = Guid.Parse(dealerIdStr);
                var (ok, err, data) = await _customerService.GetAllByDealerAsync(dealerId);
                if (!ok)
                {
                    TempData["Error"] = err;
                    return View(new List<Customer>());
                }
                customers = data;
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                customers = customers.Where(c =>
                    c.FullName.ToLower().Contains(searchLower) ||
                    c.PhoneNumber.Contains(search) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchLower))
                ).ToList();
            }

            ViewBag.Search = search;
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var (ok, err, customer) = await _customerService.GetAsync(id);
            if (!ok)
            {
                TempData["Error"] = err ?? "Không tìm thấy khách hàng";
                return RedirectToAction(nameof(Index));
            }

            // Lấy lịch sử đơn hàng
            var orders = await _context.Order
                .Include(o => o.Product)
                .Include(o => o.Dealer)
                .Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Lấy lịch sử lái thử
            var testDrives = await _context.TestDrive
                .Include(td => td.Product)
                .Include(td => td.Dealer)
                .Where(td => td.CustomerEmail == customer.Email || td.CustomerPhone == customer.PhoneNumber)
                .OrderByDescending(td => td.CreatedAt)
                .ToListAsync();

            ViewBag.Orders = orders;
            ViewBag.TestDrives = testDrives;
            ViewBag.TotalSpent = orders.Sum(o => o.FinalAmount);
            ViewBag.TotalOrders = orders.Count;

            return View(customer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateFromTestDrive(string fullName = null, string email = null, string phoneNumber = null, string address = null)
        {
            // Pre-fill form với dữ liệu từ TestDrive nếu có
            ViewBag.PreFillData = new
            {
                FullName = fullName ?? "",
                Email = email ?? "",
                PhoneNumber = phoneNumber ?? "",
                Address = address ?? ""
            };
            
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string fullName, string email, string phoneNumber, string address = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (ok, err, customer) = await _customerService.CreateAsync(fullName, email, phoneNumber, address);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                return View();
            }

            TempData["Msg"] = "Thêm khách hàng thành công!";
            
            // Nếu đến từ TestDrive, redirect về TestDrive thay vì Detail
            if (Request.Headers["Referer"].ToString().Contains("TestDrive"))
            {
                TempData["Success"] = "Khách hàng đã được tạo thành công từ lịch lái thử!";
                return RedirectToAction("Index", "TestDrive");
            }
            
            return RedirectToAction(nameof(Detail), new { id = customer.Id });
        }
    }
}

