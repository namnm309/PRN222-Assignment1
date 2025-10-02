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
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _repo;

        public DealerService(IDealerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DealerDto>> GetAllAsync()
        {
            var dealers = await _repo.GetAllAsync();
            return dealers.Select(d => new DealerDto
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.phone,
                Address = d.Address,
                IsActive = d.IsActive
            });
        }

        public async Task<DealerDto> GetByIdAsync(Guid id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            return new DealerDto
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.phone,
                Address = d.Address,
                IsActive = d.IsActive
            };
        }

        public async Task<DealerDto> CreateAsync(CreateDealerDto dto)
        {
            var dealer = new Dealer
            {
                Name = dto.Name,
                phone = dto.Phone,
                Address = dto.Address,
                IsActive = true
            };

            await _repo.AddAsync(dealer);

            return new DealerDto
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Phone = dealer.phone,
                Address = dealer.Address,
                IsActive = dealer.IsActive
            };
        }

        public async Task<DealerDto> UpdateAsync(Guid id, DealerDto dto)
        {
            var dealer = await _repo.GetByIdAsync(id);
            if (dealer == null) return null;

            dealer.Name = dto.Name;
            dealer.phone = dto.Phone;
            dealer.Address = dto.Address;
            dealer.IsActive = dto.IsActive;

            await _repo.UpdateAsync(dealer);

            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var dealer = await _repo.GetByIdAsync(id);
            if (dealer == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
