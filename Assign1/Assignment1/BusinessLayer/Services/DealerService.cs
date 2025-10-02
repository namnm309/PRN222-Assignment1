using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _repo;

        public DealerService(IDealerRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Dealer>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Dealer> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task CreateAsync(Dealer dealer)
        {
            dealer.Id = Guid.NewGuid();
            await _repo.AddAsync(dealer);
        }

        public async Task UpdateAsync(Guid id, Dealer dealer)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;

            existing.Name = dealer.Name;
            existing.Address = dealer.Address;
            existing.phone = dealer.phone;
            existing.IsActive = dealer.IsActive;

            await _repo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var dealer = await _repo.GetByIdAsync(id);
            if (dealer == null) return false;

            await _repo.DeleteAsync(id); // gọi theo Id
            return true;
        }
    }
}
