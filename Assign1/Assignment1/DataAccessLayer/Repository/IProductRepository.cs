using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<List<Product>> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive);
    }
}
