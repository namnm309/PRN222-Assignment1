using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IProductService
    {
        Task<(bool Success, string Error, Product Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, List<Product> Data)> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive = true);
    }
}
