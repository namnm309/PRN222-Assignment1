using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.DTOs;

namespace BusinessLayer.Services
{
    public class EVMReportService : IEVMReportService
    {
        private readonly AppDbContext _dbContext;

        public EVMReportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EVMSalesReportDTO>> GetSalesReportByRegionAsync(EVMSalesReportFilterDTO filter)
        {
            var query = _dbContext.Order
                .Include(o => o.Dealer)
                .Include(o => o.Region)
                .Where(o => o.Status == "Completed" || o.Status == "Delivered");

            // Apply filters
            if (!string.IsNullOrEmpty(filter.RegionId) && Guid.TryParse(filter.RegionId, out var regionId))
            {
                query = query.Where(o => o.RegionId == regionId);
            }

            if (!string.IsNullOrEmpty(filter.DealerId) && Guid.TryParse(filter.DealerId, out var dealerId))
            {
                query = query.Where(o => o.DealerId == dealerId);
            }

            // Apply date filters based on period
            var now = DateTime.UtcNow;
            switch (filter.Period?.ToLower())
            {
                case "monthly":
                    if (filter.Month.HasValue)
                    {
                        query = query.Where(o => o.OrderDate.Value.Year == filter.Year && o.OrderDate.Value.Month == filter.Month.Value);
                    }
                    break;
                case "quarterly":
                    if (filter.Quarter.HasValue)
                    {
                        var quarterStartMonth = (filter.Quarter.Value - 1) * 3 + 1;
                        var quarterEndMonth = quarterStartMonth + 2;
                        query = query.Where(o => o.OrderDate.Value.Year == filter.Year && 
                                               o.OrderDate.Value.Month >= quarterStartMonth && 
                                               o.OrderDate.Value.Month <= quarterEndMonth);
                    }
                    break;
                case "yearly":
                    query = query.Where(o => o.OrderDate.Value.Year == filter.Year);
                    break;
            }

            var result = await query
                .GroupBy(o => new { o.RegionId, RegionName = o.Region.Name, o.DealerId, DealerName = o.Dealer.Name, o.Dealer.DealerCode })
                .Select(g => new EVMSalesReportDTO
                {
                    RegionName = g.Key.RegionName ?? "N/A",
                    DealerName = g.Key.DealerName ?? "N/A",
                    DealerCode = g.Key.DealerCode ?? "N/A",
                    TotalSales = g.Sum(o => o.FinalAmount),
                    TotalOrders = g.Count(),
                    AverageOrderValue = g.Average(o => o.FinalAmount),
                    AchievementRate = 0, // Will be calculated separately
                    Period = filter.Period,
                    ReportDate = now
                })
                .OrderByDescending(r => r.TotalSales)
                .ToListAsync();

            return result;
        }

        public async Task<List<EVMSalesReportDTO>> GetSalesReportByDealerAsync(EVMSalesReportFilterDTO filter)
        {
            return await GetSalesReportByRegionAsync(filter);
        }

        public async Task<decimal> GetTotalSalesAsync(EVMSalesReportFilterDTO filter)
        {
            var query = _dbContext.Order
                .Where(o => o.Status == "Completed" || o.Status == "Delivered");

            // Apply same filters as GetSalesReportByRegionAsync
            if (!string.IsNullOrEmpty(filter.RegionId) && Guid.TryParse(filter.RegionId, out var regionId))
            {
                query = query.Where(o => o.RegionId == regionId);
            }

            if (!string.IsNullOrEmpty(filter.DealerId) && Guid.TryParse(filter.DealerId, out var dealerId))
            {
                query = query.Where(o => o.DealerId == dealerId);
            }

            return await query.SumAsync(o => o.FinalAmount);
        }

        public async Task<List<EVMInventoryDTO>> GetInventoryReportAsync(EVMInventoryFilterDTO filter)
        {
            var query = _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(filter.ProductId) && Guid.TryParse(filter.ProductId, out var productId))
            {
                query = query.Where(p => p.Id == productId);
            }

            if (!string.IsNullOrEmpty(filter.BrandId) && Guid.TryParse(filter.BrandId, out var brandId))
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            var products = await query.ToListAsync();

            var result = products.Select(p => new EVMInventoryDTO
            {
                ProductName = p.Name,
                ProductSku = p.Sku,
                BrandName = p.Brand?.Name ?? "N/A",
                CurrentStock = p.StockQuantity,
                MinStockLevel = 5, // Default minimum stock level
                MaxStockLevel = 50, // Default maximum stock level
                ConsumptionRate = CalculateConsumptionRate(p.Id),
                DaysToStockOut = p.StockQuantity > 0 ? (int)(p.StockQuantity / Math.Max(CalculateConsumptionRate(p.Id), 0.1m)) : 0,
                StockStatus = GetStockStatus(p.StockQuantity, 5, 50),
                TurnoverRate = CalculateTurnoverRate(p.Id),
                LastUpdated = p.UpdatedAt
            }).ToList();

            // Apply additional filters
            if (!string.IsNullOrEmpty(filter.StockStatus))
            {
                result = result.Where(r => r.StockStatus == filter.StockStatus).ToList();
            }

            if (filter.MinStockLevel.HasValue)
            {
                result = result.Where(r => r.CurrentStock >= filter.MinStockLevel.Value).ToList();
            }

            if (filter.MaxStockLevel.HasValue)
            {
                result = result.Where(r => r.CurrentStock <= filter.MaxStockLevel.Value).ToList();
            }

            return result.OrderByDescending(r => r.DaysToStockOut).ToList();
        }

        public async Task<List<EVMInventoryDTO>> GetLowStockProductsAsync()
        {
            var filter = new EVMInventoryFilterDTO { StockStatus = "Low" };
            return await GetInventoryReportAsync(filter);
        }

        public async Task<List<EVMInventoryDTO>> GetCriticalStockProductsAsync()
        {
            var filter = new EVMInventoryFilterDTO { StockStatus = "Critical" };
            return await GetInventoryReportAsync(filter);
        }

        public async Task<List<EVMDemandForecastDTO>> GetDemandForecastAsync(EVMDemandForecastFilterDTO filter)
        {
            // This is a simplified AI prediction - in real implementation, this would use ML models
            var query = _dbContext.Product
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(filter.ProductId) && Guid.TryParse(filter.ProductId, out var productId))
            {
                query = query.Where(p => p.Id == productId);
            }

            if (!string.IsNullOrEmpty(filter.BrandId) && Guid.TryParse(filter.BrandId, out var brandId))
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            var products = await query.ToListAsync();

            var result = products.Select(p => new EVMDemandForecastDTO
            {
                ProductName = p.Name,
                ProductSku = p.Sku,
                BrandName = p.Brand?.Name ?? "N/A",
                RegionName = "Toàn quốc", // Simplified for now
                CurrentDemand = GetCurrentDemand(p.Id),
                PredictedDemand = PredictDemand(p.Id, filter.ForecastPeriod),
                ConfidenceLevel = 0.85m, // Simulated confidence level
                Trend = GetTrend(p.Id),
                GrowthRate = GetGrowthRate(p.Id),
                RecommendedProduction = PredictDemand(p.Id, filter.ForecastPeriod),
                RecommendedDistribution = PredictDemand(p.Id, filter.ForecastPeriod) / 3, // Distribute to 3 regions
                ForecastDate = DateTime.UtcNow,
                Priority = GetPriority(p.Id)
            }).ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Priority))
            {
                result = result.Where(r => r.Priority == filter.Priority).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Trend))
            {
                result = result.Where(r => r.Trend == filter.Trend).ToList();
            }

            return result.OrderByDescending(r => r.PredictedDemand).ToList();
        }

        public async Task<List<EVMDemandForecastDTO>> GetHighPriorityForecastsAsync()
        {
            var filter = new EVMDemandForecastFilterDTO { Priority = "High" };
            return await GetDemandForecastAsync(filter);
        }

        public async Task<List<EVMContractManagementDTO>> GetContractManagementReportAsync(EVMContractFilterDTO filter)
        {
            var query = _dbContext.DealerContract
                .Include(dc => dc.Dealer)
                .ThenInclude(d => d.Region)
                .Where(dc => dc.IsActive);

            if (!string.IsNullOrEmpty(filter.DealerId) && Guid.TryParse(filter.DealerId, out var dealerId))
            {
                query = query.Where(dc => dc.DealerId == dealerId);
            }

            if (!string.IsNullOrEmpty(filter.RegionId) && Guid.TryParse(filter.RegionId, out var regionId))
            {
                query = query.Where(dc => dc.Dealer.RegionId == regionId);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(dc => dc.Status == filter.Status);
            }

            if (filter.StartDateFrom.HasValue)
            {
                query = query.Where(dc => dc.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                query = query.Where(dc => dc.StartDate <= filter.StartDateTo.Value);
            }

            var contracts = await query.ToListAsync();

            var result = contracts.Select(dc => new EVMContractManagementDTO
            {
                ContractNumber = dc.ContractNumber,
                DealerName = dc.Dealer?.Name ?? "N/A",
                DealerCode = dc.Dealer?.DealerCode ?? "N/A",
                RegionName = dc.Dealer?.Region?.Name ?? "N/A",
                StartDate = dc.StartDate,
                EndDate = dc.EndDate,
                Status = dc.Status,
                CommissionRate = dc.CommissionRate,
                CreditLimit = dc.CreditLimit,
                OutstandingDebt = dc.OutstandingDebt,
                CreditUtilization = dc.CreditLimit > 0 ? (dc.OutstandingDebt / dc.CreditLimit) * 100 : 0,
                SalesTarget = GetSalesTarget(dc.DealerId),
                ActualSales = GetActualSales(dc.DealerId),
                AchievementRate = GetAchievementRate(dc.DealerId),
                DaysToExpiry = (dc.EndDate - DateTime.UtcNow).Days,
                RiskLevel = GetRiskLevel(dc),
                LastUpdated = dc.UpdatedAt
            }).ToList();

            // Apply additional filters
            if (!string.IsNullOrEmpty(filter.RiskLevel))
            {
                result = result.Where(r => r.RiskLevel == filter.RiskLevel).ToList();
            }

            return result.OrderByDescending(r => r.RiskLevel).ThenBy(r => r.DaysToExpiry).ToList();
        }

        public async Task<List<EVMContractManagementDTO>> GetExpiringContractsAsync(int daysToExpiry = 30)
        {
            var filter = new EVMContractFilterDTO
            {
                EndDateFrom = DateTime.UtcNow,
                EndDateTo = DateTime.UtcNow.AddDays(daysToExpiry)
            };
            return await GetContractManagementReportAsync(filter);
        }

        public async Task<List<EVMContractManagementDTO>> GetHighRiskContractsAsync()
        {
            var filter = new EVMContractFilterDTO { RiskLevel = "High" };
            return await GetContractManagementReportAsync(filter);
        }

        // Helper methods
        private decimal CalculateConsumptionRate(Guid productId)
        {
            // Simplified calculation - in real implementation, this would analyze historical data
            return 2.5m; // Average units per day
        }

        private string GetStockStatus(int currentStock, int minLevel, int maxLevel)
        {
            if (currentStock <= 0) return "OutOfStock";
            if (currentStock <= minLevel) return "Critical";
            if (currentStock <= minLevel * 2) return "Low";
            return "Normal";
        }

        private decimal CalculateTurnoverRate(Guid productId)
        {
            // Simplified calculation
            return 12.5m; // Times per year
        }

        private int GetCurrentDemand(Guid productId)
        {
            // Simplified - would calculate from recent orders
            return 15;
        }

        private int PredictDemand(Guid productId, int forecastPeriod)
        {
            // Simplified AI prediction
            var baseDemand = GetCurrentDemand(productId);
            var growthFactor = 1.1m; // 10% growth
            return (int)(baseDemand * Math.Pow((double)growthFactor, forecastPeriod));
        }

        private string GetTrend(Guid productId)
        {
            // Simplified trend analysis
            return "Increasing";
        }

        private decimal GetGrowthRate(Guid productId)
        {
            return 10.5m; // 10.5% growth rate
        }

        private string GetPriority(Guid productId)
        {
            // Simplified priority calculation
            return "High";
        }

        private decimal GetSalesTarget(Guid dealerId)
        {
            // Get from SalesTarget table
            return 1000000m; // 1M VND
        }

        private decimal GetActualSales(Guid dealerId)
        {
            // Calculate from orders
            return 850000m; // 850K VND
        }

        private decimal GetAchievementRate(Guid dealerId)
        {
            var target = GetSalesTarget(dealerId);
            var actual = GetActualSales(dealerId);
            return target > 0 ? (actual / target) * 100 : 0;
        }

        private string GetRiskLevel(DealerContract contract)
        {
            var creditUtilization = contract.CreditLimit > 0 ? (contract.OutstandingDebt / contract.CreditLimit) * 100 : 0;
            var daysToExpiry = (contract.EndDate - DateTime.UtcNow).Days;

            if (creditUtilization > 80 || daysToExpiry < 30)
                return "High";
            if (creditUtilization > 60 || daysToExpiry < 90)
                return "Medium";
            return "Low";
        }
    }
}
