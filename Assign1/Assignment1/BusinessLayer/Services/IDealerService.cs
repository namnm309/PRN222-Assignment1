using BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IDealerService
    {
        Task<IEnumerable<DealerDto>> GetAllAsync();
        Task<DealerDto> GetByIdAsync(Guid id);
        Task<DealerDto> CreateAsync(CreateDealerDto dto);
        Task<DealerDto> UpdateAsync(Guid id, DealerDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
