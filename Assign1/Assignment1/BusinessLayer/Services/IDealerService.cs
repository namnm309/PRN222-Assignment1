using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IDealerService
    {
        Task<IEnumerable<Dealer>> GetAllAsync();
        Task<Dealer> GetByIdAsync(Guid id);
        Task CreateAsync(Dealer dealer);
        Task UpdateAsync(Guid id, Dealer dealer);
        Task<bool> DeleteAsync(Guid id);
    }
}
