using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Services
{
    public interface IEVMReportService
    {
        // Sales Reports
        Task<List<EVMSalesReportDTO>> GetSalesReportByRegionAsync(EVMSalesReportFilterDTO filter);
        Task<List<EVMSalesReportDTO>> GetSalesReportByDealerAsync(EVMSalesReportFilterDTO filter);
        Task<decimal> GetTotalSalesAsync(EVMSalesReportFilterDTO filter);
        
        // Inventory Reports
        Task<List<EVMInventoryDTO>> GetInventoryReportAsync(EVMInventoryFilterDTO filter);
        Task<List<EVMInventoryDTO>> GetLowStockProductsAsync();
        Task<List<EVMInventoryDTO>> GetCriticalStockProductsAsync();
        
        // Demand Forecast
        Task<List<EVMDemandForecastDTO>> GetDemandForecastAsync(EVMDemandForecastFilterDTO filter);
        Task<List<EVMDemandForecastDTO>> GetHighPriorityForecastsAsync();
        
        // Contract Management
        Task<List<EVMContractManagementDTO>> GetContractManagementReportAsync(EVMContractFilterDTO filter);
        Task<List<EVMContractManagementDTO>> GetExpiringContractsAsync(int daysToExpiry = 30);
        Task<List<EVMContractManagementDTO>> GetHighRiskContractsAsync();
    }
}
