using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Product : BaseEntity
    {
        public string Sku { get; set; } // Stock Keeping Unit
        
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        //Qhe n-1 với Brand
        public virtual Brand BrandName { get; set; }
    }
}
