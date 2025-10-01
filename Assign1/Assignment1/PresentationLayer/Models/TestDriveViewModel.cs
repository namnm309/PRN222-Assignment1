using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class TestDriveViewModel
    {
        [Required] public Guid CustomerId { get; set; }
        [Required] public Guid ProductId { get; set; }
        [Required] public Guid DealerId { get; set; }
        [Required] public DateTime ScheduledDate { get; set; }
    }
}
