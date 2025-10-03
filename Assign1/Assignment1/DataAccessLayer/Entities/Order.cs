using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Order : BaseEntity
    {
        
        public Guid CustomerId { get; set; }
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? SalesPersonId { get; set; } 

        public virtual Customer Customer { get; set; }  
        public virtual Dealer Dealer { get; set; } 
        public virtual Product Product { get; set; } 
        public virtual Region Region { get; set; } 
        public virtual Users SalesPerson { get; set; } 

        public string OrderNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = "Pending"; 
        public string PaymentStatus { get; set; } = "Pending";
        public string PaymentMethod { get; set; } 
        
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        
        public string Notes { get; set; }
    }

}
