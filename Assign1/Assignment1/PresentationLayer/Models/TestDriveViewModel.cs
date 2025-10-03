using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class TestDriveViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [Required] 
        public Guid ProductId { get; set; }
        
        [Required] 
        public Guid DealerId { get; set; }
        
        [Display(Name = "Thời gian hẹn")]
        public DateTime ScheduledDate { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
