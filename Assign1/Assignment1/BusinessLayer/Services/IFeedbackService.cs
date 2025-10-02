using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IFeedbackService
    {
        Task<(bool Success, string Error, Feedback Data)> CreateAsync(Guid customerId, Guid productId, string comment, int rating);
        Task<(bool Success, string Error, List<Feedback> Data)> GetByProductAsync(Guid productId);
        Task<(bool Success, string Error)> DeleteAsync(Guid id);
    }
}
