using System;

namespace BusinessLayer.ViewModels
{
    public class InventoryAllocationViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid DealerId { get; set; }
        public int AllocatedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
    }
}
