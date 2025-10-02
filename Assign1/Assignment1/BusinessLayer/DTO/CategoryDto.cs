using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string ModelName { get; set; }
        public string Color { get; set; }
        public string Varian { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCategoryDto
    {
        public string ModelName { get; set; }
        public string Color { get; set; }
        public string Varian { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
