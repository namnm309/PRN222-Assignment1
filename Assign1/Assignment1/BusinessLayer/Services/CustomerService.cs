using System;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        public CustomerService(ICustomerRepository repo) => _repo = repo;

        public async Task<(bool Success, string Error, Customer Data)> GetAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            return c == null ? (false, "Không tìm thấy", null) : (true, null, c);
        }

        public async Task<(bool Success, string Error, Customer Data)> UpdateProfileAsync(Customer updated)
        {
            if (updated == null || updated.Id == Guid.Empty) return (false, "Thiếu dữ liệu", null);
            updated.UpdatedAt = DateTime.UtcNow;
            var ok = await _repo.UpdateAsync(updated);
            return ok ? (true, null, updated) : (false, "Cập nhật thất bại", null);
        }

        public async Task<(bool Success, string Error, Customer Data)> CreateAsync(string fullName, string email, string phoneNumber, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return (false, "Vui lòng nhập họ tên", null);
            if (string.IsNullOrWhiteSpace(phoneNumber)) return (false, "Vui lòng nhập số điện thoại", null);
            if (string.IsNullOrWhiteSpace(email)) return (false, "Vui lòng nhập email", null);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Name = fullName, // Duplicate for backward compatibility
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address ?? "",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(customer);
            return ok ? (true, null, customer) : (false, "Không thể tạo khách hàng", null);
        }
    }
}
