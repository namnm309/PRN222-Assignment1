using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class TestDrive : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DealerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Dealer Dealer { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = "Successfully"; 
    }
}
