using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Category> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task CreateAsync(Category category)
        {
            category.Id = Guid.NewGuid();
            await _repo.AddAsync(category);
        }

        public async Task UpdateAsync(Guid id, Category category)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;

            existing.ModelName = category.ModelName;
            existing.color = category.color;
            existing.varian = category.varian;
            existing.IsActive = category.IsActive;

            await _repo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repo.DeleteAsync(id); // repo trả về bool
        }
    }
}
