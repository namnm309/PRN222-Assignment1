using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Repository
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetByIdAsync(Guid id);
        Task<List<Feedback>> GetByProductAsync(Guid productId);
        Task<List<Feedback>> GetByCustomerAsync(Guid customerId);
        Task<bool> CreateAsync(Feedback feedback);
        Task<bool> DeleteAsync(Guid id);
    }
}
