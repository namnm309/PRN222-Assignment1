using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IDealerRepository
    {
        Task<IEnumerable<Dealer>> GetAllAsync();
        Task<Dealer> GetByIdAsync(Guid id);
        Task AddAsync(Dealer dealer);
        Task UpdateAsync(Dealer dealer);
        Task DeleteAsync(Guid id);
    }
}
