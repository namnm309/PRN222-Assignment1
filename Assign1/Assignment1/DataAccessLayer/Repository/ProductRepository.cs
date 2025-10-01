using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) => _db = db;

        public Task<Product?> GetByIdAsync(Guid id)
            => _db.Product.Include(p => p.Brand).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<List<Product>> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive)
        {
            var query = _db.Product.Include(p => p.Brand).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term) || x.Sku.ToLower().Contains(term) || x.Description.ToLower().Contains(term));
            }
            if (brandId.HasValue) query = query.Where(x => x.BrandId == brandId.Value);
            if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice.Value);
            if (inStock == true) query = query.Where(x => x.StockQuantity > 0);
            if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive);

            return query.OrderBy(x => x.Name).ToListAsync();
        }
    }
}
