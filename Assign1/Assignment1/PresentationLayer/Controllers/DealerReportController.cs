using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;

namespace PresentationLayer.Controllers
{
    public class DealerReportController : BaseDashboardController
    {
        private readonly IEVMReportService _evmReportService;
        private readonly AppDbContext _context;

        public DealerReportController(IEVMReportService evmReportService, AppDbContext context)
        {
            _evmReportService = evmReportService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dữ liệu tổng quan
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Doanh số tháng hiện tại
            var monthlySales = await GetDealerSalesAsync(dealerId, "monthly", currentYear, currentMonth);
            var monthlyTotal = monthlySales.Sum(o => o.FinalAmount);

            // Doanh số năm hiện tại
            var yearlySales = await GetDealerSalesAsync(dealerId, "yearly", currentYear);
            var yearlyTotal = yearlySales.Sum(o => o.FinalAmount);

            // Top nhân viên bán hàng tháng này
            var topEmployees = await GetTopEmployeesAsync(dealerId, "monthly", currentYear, currentMonth);

            // Top xe bán chạy tháng này
            var topVehicles = await GetTopVehiclesAsync(dealerId, "monthly", currentYear, currentMonth);

            ViewBag.MonthlyTotal = monthlyTotal;
            ViewBag.YearlyTotal = yearlyTotal;
            ViewBag.TopEmployees = topEmployees;
            ViewBag.TopVehicles = topVehicles;
            ViewBag.CurrentYear = currentYear;
            ViewBag.CurrentMonth = currentMonth;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SalesByEmployee(Guid? employeeId = null, string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var salesReport = await GetDealerSalesByEmployeeAsync(dealerId, employeeId, period, year, month, quarter);
            var totalSales = salesReport.Sum(o => o.FinalAmount);

            ViewBag.TotalSales = totalSales;
            ViewBag.EmployeeId = employeeId;
            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            // Get dropdown data - chỉ nhân viên của dealer này
            ViewBag.Employees = await GetDealerEmployeesAsync(dealerId);

            return View(salesReport);
        }

        [HttpGet]
        public async Task<IActionResult> TopSellingVehicles(string period = "monthly", int year = 0, int? month = null, int? quarter = null)
        {
            if (!IsDealer())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy dealerId của user hiện tại
            var dealerIdStr = HttpContext.Session.GetString("DealerId");
            if (!Guid.TryParse(dealerIdStr, out var dealerId))
            {
                TempData["Error"] = "Không tìm thấy thông tin đại lý.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values
            if (year == 0)
                year = DateTime.Now.Year;
            if (month == null && period == "monthly")
                month = DateTime.Now.Month;

            var topVehicles = await GetTopVehiclesAsync(dealerId, period, year, month, quarter);

            ViewBag.Period = period;
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Quarter = quarter;

            return View(topVehicles);
        }

        // Private helper methods
        private async Task<List<Order>> GetDealerSalesAsync(Guid dealerId, string period, int year, int? month = null, int? quarter = null)
        {
            var query = _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Include(o => o.Dealer)
                .Include(o => o.SalesPerson)
                .Where(o => o.DealerId == dealerId);

            // Apply period filter
            switch (period.ToLower())
            {
                case "monthly":
                    if (month.HasValue)
                    {
                        query = query.Where(o => o.OrderDate.Value.Year == year && o.OrderDate.Value.Month == month.Value);
                    }
                    break;
                case "quarterly":
                    if (quarter.HasValue)
                    {
                        var startMonth = (quarter.Value - 1) * 3 + 1;
                        var endMonth = quarter.Value * 3;
                        query = query.Where(o => o.OrderDate.Value.Year == year && 
                                               o.OrderDate.Value.Month >= startMonth && 
                                               o.OrderDate.Value.Month <= endMonth);
                    }
                    break;
                case "yearly":
                    query = query.Where(o => o.OrderDate.Value.Year == year);
                    break;
            }

            return await query.ToListAsync();
        }

        private async Task<List<Order>> GetDealerSalesByEmployeeAsync(Guid dealerId, Guid? employeeId, string period, int year, int? month = null, int? quarter = null)
        {
            var query = _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Include(o => o.Dealer)
                .Include(o => o.SalesPerson)
                .Where(o => o.DealerId == dealerId);

            if (employeeId.HasValue)
            {
                query = query.Where(o => o.SalesPersonId == employeeId.Value);
            }

            // Apply period filter
            switch (period.ToLower())
            {
                case "monthly":
                    if (month.HasValue)
                    {
                        query = query.Where(o => o.OrderDate.Value.Year == year && o.OrderDate.Value.Month == month.Value);
                    }
                    break;
                case "quarterly":
                    if (quarter.HasValue)
                    {
                        var startMonth = (quarter.Value - 1) * 3 + 1;
                        var endMonth = quarter.Value * 3;
                        query = query.Where(o => o.OrderDate.Value.Year == year && 
                                               o.OrderDate.Value.Month >= startMonth && 
                                               o.OrderDate.Value.Month <= endMonth);
                    }
                    break;
                case "yearly":
                    query = query.Where(o => o.OrderDate.Value.Year == year);
                    break;
            }

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        private async Task<List<dynamic>> GetTopEmployeesAsync(Guid dealerId, string period, int year, int? month = null, int? quarter = null)
        {
            var sales = await GetDealerSalesAsync(dealerId, period, year, month, quarter);
            
            return sales
                .Where(o => o.SalesPersonId.HasValue)
                .GroupBy(o => new { o.SalesPersonId, o.SalesPerson.FullName })
                .Select(g => new
                {
                    EmployeeId = g.Key.SalesPersonId,
                    EmployeeName = g.Key.FullName,
                    TotalSales = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .Cast<dynamic>()
                .ToList();
        }

        private async Task<List<dynamic>> GetTopVehiclesAsync(Guid dealerId, string period, int year, int? month = null, int? quarter = null)
        {
            var sales = await GetDealerSalesAsync(dealerId, period, year, month, quarter);
            
            return sales
                .GroupBy(o => new { o.ProductId, o.Product.Name, o.Product.Sku })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    ProductSku = g.Key.Sku,
                    TotalSales = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count(),
                    AveragePrice = g.Average(o => o.FinalAmount)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .Cast<dynamic>()
                .ToList();
        }

        private async Task<List<Users>> GetDealerEmployeesAsync(Guid dealerId)
        {
            return await _context.Users
                .Where(u => u.DealerId == dealerId && u.Role == UserRole.DealerStaff)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}
