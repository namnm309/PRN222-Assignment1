using System;
using DataAccessLayer.Enum;

namespace DataAccessLayer.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public Guid DealerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid RequestedById { get; set; } 
        public Guid? ApprovedById { get; set; } 
        
        public virtual Dealer Dealer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Users RequestedBy { get; set; }
        public virtual Users? ApprovedBy { get; set; }

        public string OrderNumber { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal TotalAmount { get; set; }
        
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        
        public string Reason { get; set; } = string.Empty; 
        public string Notes { get; set; } = string.Empty;
        public string? RejectReason { get; set; } 
    }
}
