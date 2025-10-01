using BusinessLayer.DTOs;
using PresentationLayer.Models;

namespace PresentationLayer.Extensions
{
    public static class MappingExtensions
    {
        // Sales Report Mappings
        public static EVMSalesReportViewModel ToViewModel(this EVMSalesReportDTO dto)
        {
            return new EVMSalesReportViewModel
            {
                RegionName = dto.RegionName,
                DealerName = dto.DealerName,
                DealerCode = dto.DealerCode,
                TotalSales = dto.TotalSales,
                TotalOrders = dto.TotalOrders,
                AverageOrderValue = dto.AverageOrderValue,
                AchievementRate = dto.AchievementRate,
                Period = dto.Period,
                ReportDate = dto.ReportDate
            };
        }

        public static EVMSalesReportFilterDTO ToDTO(this EVMSalesReportFilterViewModel viewModel)
        {
            return new EVMSalesReportFilterDTO
            {
                RegionId = viewModel.RegionId,
                DealerId = viewModel.DealerId,
                Period = viewModel.Period,
                Year = viewModel.Year,
                Month = viewModel.Month,
                Quarter = viewModel.Quarter,
                Priority = viewModel.Priority
            };
        }

        // Inventory Mappings
        public static EVMInventoryViewModel ToViewModel(this EVMInventoryDTO dto)
        {
            return new EVMInventoryViewModel
            {
                ProductName = dto.ProductName,
                ProductSku = dto.ProductSku,
                BrandName = dto.BrandName,
                CurrentStock = dto.CurrentStock,
                MinStockLevel = dto.MinStockLevel,
                MaxStockLevel = dto.MaxStockLevel,
                ConsumptionRate = dto.ConsumptionRate,
                DaysToStockOut = dto.DaysToStockOut,
                StockStatus = dto.StockStatus,
                TurnoverRate = dto.TurnoverRate,
                LastUpdated = dto.LastUpdated
            };
        }

        public static EVMInventoryFilterDTO ToDTO(this EVMInventoryFilterViewModel viewModel)
        {
            return new EVMInventoryFilterDTO
            {
                ProductId = viewModel.ProductId,
                BrandId = viewModel.BrandId,
                StockStatus = viewModel.StockStatus,
                Priority = viewModel.Priority,
                MinStockLevel = viewModel.MinStockLevel,
                MaxStockLevel = viewModel.MaxStockLevel
            };
        }

        // Demand Forecast Mappings
        public static EVMDemandForecastViewModel ToViewModel(this EVMDemandForecastDTO dto)
        {
            return new EVMDemandForecastViewModel
            {
                ProductName = dto.ProductName,
                ProductSku = dto.ProductSku,
                BrandName = dto.BrandName,
                RegionName = dto.RegionName,
                CurrentDemand = dto.CurrentDemand,
                PredictedDemand = dto.PredictedDemand,
                ConfidenceLevel = dto.ConfidenceLevel,
                Trend = dto.Trend,
                GrowthRate = dto.GrowthRate,
                RecommendedProduction = dto.RecommendedProduction,
                RecommendedDistribution = dto.RecommendedDistribution,
                ForecastDate = dto.ForecastDate,
                Priority = dto.Priority
            };
        }

        public static EVMDemandForecastFilterDTO ToDTO(this EVMDemandForecastFilterViewModel viewModel)
        {
            return new EVMDemandForecastFilterDTO
            {
                ProductId = viewModel.ProductId,
                BrandId = viewModel.BrandId,
                RegionId = viewModel.RegionId,
                Priority = viewModel.Priority,
                ForecastPeriod = viewModel.ForecastPeriod,
                Trend = viewModel.Trend
            };
        }

        // Contract Management Mappings
        public static EVMContractManagementViewModel ToViewModel(this EVMContractManagementDTO dto)
        {
            return new EVMContractManagementViewModel
            {
                ContractNumber = dto.ContractNumber,
                DealerName = dto.DealerName,
                DealerCode = dto.DealerCode,
                RegionName = dto.RegionName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                CommissionRate = dto.CommissionRate,
                CreditLimit = dto.CreditLimit,
                OutstandingDebt = dto.OutstandingDebt,
                CreditUtilization = dto.CreditUtilization,
                SalesTarget = dto.SalesTarget,
                ActualSales = dto.ActualSales,
                AchievementRate = dto.AchievementRate,
                DaysToExpiry = dto.DaysToExpiry,
                RiskLevel = dto.RiskLevel,
                LastUpdated = dto.LastUpdated
            };
        }

        public static EVMContractFilterDTO ToDTO(this EVMContractFilterViewModel viewModel)
        {
            return new EVMContractFilterDTO
            {
                DealerId = viewModel.DealerId,
                RegionId = viewModel.RegionId,
                Status = viewModel.Status,
                RiskLevel = viewModel.RiskLevel,
                Priority = viewModel.Priority,
                StartDateFrom = viewModel.StartDateFrom,
                StartDateTo = viewModel.StartDateTo,
                EndDateFrom = viewModel.EndDateFrom,
                EndDateTo = viewModel.EndDateTo
            };
        }
    }
}
