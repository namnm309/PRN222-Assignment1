using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo) => _repo = repo;

        public async Task<(bool Success, string Error, Product Data)> GetAsync(Guid id)
        {
            var p = await _repo.GetByIdAsync(id);
            return p == null ? (false, "Không tìm thấy", null) : (true, null, p);
        }

        public Task<(bool Success, string Error, List<Product> Data)> SearchAsync(string? q, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? inStock, bool? isActive = true)
            => Execute();

        private async Task<(bool Success, string Error, List<Product> Data)> Execute(string? q = null, Guid? brandId = null, decimal? minPrice = null, decimal? maxPrice = null, bool? inStock = null, bool? isActive = true)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return (false, "Khoảng giá không hợp lệ", null);
            var list = await _repo.SearchAsync(q, brandId, minPrice, maxPrice, inStock, isActive);
            return (true, null, list);
        }
    }
}
