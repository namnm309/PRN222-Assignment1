using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using PresentationLayer.Models;
using PresentationLayer.Extensions;
using DataAccessLayer.Enum;

namespace PresentationLayer.Controllers
{
    public class EVMDashboardController : BaseDashboardController
    {
        private readonly IEVMReportService _evmReportService;

        public EVMDashboardController(IEVMReportService evmReportService)
        {
            _evmReportService = evmReportService;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "EVM Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SalesReport(EVMSalesReportFilterViewModel filter)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values if not provided
            if (string.IsNullOrEmpty(filter.Period))
                filter.Period = "monthly";
            if (filter.Year == 0)
                filter.Year = DateTime.Now.Year;
            if (filter.Month == null)
                filter.Month = DateTime.Now.Month;

            var filterDto = filter.ToDTO();
            var salesReportDto = await _evmReportService.GetSalesReportByRegionAsync(filterDto);
            var totalSales = await _evmReportService.GetTotalSalesAsync(filterDto);

            var salesReport = salesReportDto.Select(dto => dto.ToViewModel()).ToList();

            ViewBag.TotalSales = totalSales;
            ViewBag.Filter = filter;

            return View(salesReport);
        }

        [HttpGet]
        public async Task<IActionResult> InventoryReport(EVMInventoryFilterViewModel filter)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var filterDto = filter.ToDTO();
            var inventoryReportDto = await _evmReportService.GetInventoryReportAsync(filterDto);
            var lowStockProductsDto = await _evmReportService.GetLowStockProductsAsync();
            var criticalStockProductsDto = await _evmReportService.GetCriticalStockProductsAsync();

            var inventoryReport = inventoryReportDto.Select(dto => dto.ToViewModel()).ToList();

            ViewBag.LowStockCount = lowStockProductsDto.Count;
            ViewBag.CriticalStockCount = criticalStockProductsDto.Count;
            ViewBag.Filter = filter;

            return View(inventoryReport);
        }

        [HttpGet]
        public async Task<IActionResult> DemandForecast(EVMDemandForecastFilterViewModel filter)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set default values
            if (filter.ForecastPeriod == 0)
                filter.ForecastPeriod = 6; // 6 months forecast

            var filterDto = filter.ToDTO();
            var demandForecastDto = await _evmReportService.GetDemandForecastAsync(filterDto);
            var highPriorityForecastsDto = await _evmReportService.GetHighPriorityForecastsAsync();

            var demandForecast = demandForecastDto.Select(dto => dto.ToViewModel()).ToList();

            ViewBag.HighPriorityCount = highPriorityForecastsDto.Count;
            ViewBag.Filter = filter;

            return View(demandForecast);
        }

        [HttpGet]
        public async Task<IActionResult> ContractManagement(EVMContractFilterViewModel filter)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            var filterDto = filter.ToDTO();
            var contractReportDto = await _evmReportService.GetContractManagementReportAsync(filterDto);
            var expiringContractsDto = await _evmReportService.GetExpiringContractsAsync(30);
            var highRiskContractsDto = await _evmReportService.GetHighRiskContractsAsync();

            var contractReport = contractReportDto.Select(dto => dto.ToViewModel()).ToList();

            ViewBag.ExpiringCount = expiringContractsDto.Count;
            ViewBag.HighRiskCount = highRiskContractsDto.Count;
            ViewBag.Filter = filter;

            return View(contractReport);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesData(string regionId, string dealerId, string period, int year, int? month, int? quarter)
        {
            if (!IsAdmin())
            {
                return Json(new { error = "Unauthorized" });
            }

            var filter = new EVMSalesReportFilterDTO
            {
                RegionId = regionId,
                DealerId = dealerId,
                Period = period,
                Year = year,
                Month = month,
                Quarter = quarter
            };

            var salesData = await _evmReportService.GetSalesReportByRegionAsync(filter);
            return Json(salesData);
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryData(string productId, string brandId, string stockStatus)
        {
            if (!IsAdmin())
            {
                return Json(new { error = "Unauthorized" });
            }

            var filter = new EVMInventoryFilterDTO
            {
                ProductId = productId,
                BrandId = brandId,
                StockStatus = stockStatus
            };

            var inventoryData = await _evmReportService.GetInventoryReportAsync(filter);
            return Json(inventoryData);
        }

        [HttpGet]
        public async Task<IActionResult> GetDemandForecastData(string productId, string brandId, string priority, int forecastPeriod)
        {
            if (!IsAdmin())
            {
                return Json(new { error = "Unauthorized" });
            }

            var filter = new EVMDemandForecastFilterDTO
            {
                ProductId = productId,
                BrandId = brandId,
                Priority = priority,
                ForecastPeriod = forecastPeriod
            };

            var forecastData = await _evmReportService.GetDemandForecastAsync(filter);
            return Json(forecastData);
        }

        [HttpGet]
        public async Task<IActionResult> GetContractData(string dealerId, string regionId, string status, string riskLevel)
        {
            if (!IsAdmin())
            {
                return Json(new { error = "Unauthorized" });
            }

            var filter = new EVMContractFilterDTO
            {
                DealerId = dealerId,
                RegionId = regionId,
                Status = status,
                RiskLevel = riskLevel
            };

            var contractData = await _evmReportService.GetContractManagementReportAsync(filter);
            return Json(contractData);
        }
    }
}
