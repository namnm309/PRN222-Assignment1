using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class EVMSalesReportDTO
    {
        public string RegionName { get; set; }
        public string DealerName { get; set; }
        public string DealerCode { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal AchievementRate { get; set; }
        public string Period { get; set; }
        public DateTime ReportDate { get; set; }
    }

    public class EVMSalesReportFilterDTO
    {
        public string RegionId { get; set; }
        public string DealerId { get; set; }
        public string Period { get; set; } // Monthly, Quarterly, Yearly
        public int Year { get; set; }
        public int? Month { get; set; }
        public int? Quarter { get; set; }
        public string Priority { get; set; } // High, Medium, Low
    }
}
