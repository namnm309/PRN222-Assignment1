using System;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface ICustomerService
    {
        Task<(bool Success, string Error, Customer Data)> GetAsync(Guid id);
        Task<(bool Success, string Error, Customer Data)> UpdateProfileAsync(Customer updated);
    }
}
