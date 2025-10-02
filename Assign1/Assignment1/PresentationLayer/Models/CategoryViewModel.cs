using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Model name is required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Variant is required")]
        public string Variant { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
