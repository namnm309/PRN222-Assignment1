using BusinessLayer.DTO;
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

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                ModelName = c.ModelName,
                Color = c.color,
                Varian = c.varian,
                IsActive = c.IsActive
            });
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            return new CategoryDto
            {
                Id = c.Id,
                ModelName = c.ModelName,
                Color = c.color,
                Varian = c.varian,
                IsActive = c.IsActive
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var entity = new Category
            {
                ModelName = dto.ModelName,
                color = dto.Color,
                varian = dto.Varian,
                IsActive = dto.IsActive
            };
            await _repo.AddAsync(entity);
            return new CategoryDto
            {
                Id = entity.Id,
                ModelName = entity.ModelName,
                Color = entity.color,
                Varian = entity.varian,
                IsActive = entity.IsActive
            };
        }

        public async Task<CategoryDto?> UpdateAsync(Guid id, CategoryDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ModelName = dto.ModelName;
            entity.color = dto.Color;
            entity.varian = dto.Varian;
            entity.IsActive = dto.IsActive;

            await _repo.UpdateAsync(entity);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
