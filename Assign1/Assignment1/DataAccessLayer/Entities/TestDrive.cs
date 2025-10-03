using System;
using DataAccessLayer.Enum;

namespace DataAccessLayer.Entities
{
    public class TestDrive : BaseEntity
    {
        public Guid? CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DealerId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Dealer Dealer { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public DateTime ScheduledDate { get; set; }

        public TestDriveStatus Status { get; set; } = TestDriveStatus.Pending;
    }
}
