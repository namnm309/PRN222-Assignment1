using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class DealerContract : BaseEntity
    {
        public Guid DealerId { get; set; }
        public virtual Dealer Dealer { get; set; }
        
        public string ContractNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        
        public decimal CommissionRate { get; set; } 
        public decimal CreditLimit { get; set; } 
        public decimal OutstandingDebt { get; set; } 
        
        public string Status { get; set; } = "Active"; 
        public string Terms { get; set; } 
        public string Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
