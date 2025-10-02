using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;
using DataAccessLayer.Enum;

namespace BusinessLayer.Services
{
    public class TestDriveService : ITestDriveService
    {
        private readonly ITestDriveRepository _repo;
        public TestDriveService(ITestDriveRepository repo) => _repo = repo;

        public async Task<(bool Success, string Error, TestDrive Data)> CreateAsync(Guid customerId, Guid productId, Guid dealerId, DateTime scheduledDate)
        {
            if (customerId == Guid.Empty || productId == Guid.Empty || dealerId == Guid.Empty)
                return (false, "Thiếu thông tin", null);

            if (scheduledDate < DateTime.UtcNow.AddMinutes(30))
                return (false, "Thời gian phải ít nhất 30 phút nữa", null);

            var overlaps = await _repo.GetByDealerAndProductInRangeAsync(dealerId, productId, scheduledDate, scheduledDate.AddMinutes(90));
            if (overlaps.Count > 0)
                return (false, "Trùng lịch hẹn", null);

            var td = new TestDrive
            {
                CustomerId = customerId,
                ProductId = productId,
                DealerId = dealerId,
                ScheduledDate = scheduledDate,
                Status = TestDriveStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var ok = await _repo.CreateAsync(td);
            return ok ? (true, null, td) : (false, "Không thể tạo lịch hẹn", null);
        }

        public async Task<(bool Success, string Error, TestDrive Data)> GetAsync(Guid id)
        {
            var td = await _repo.GetByIdAsync(id);
            return td == null ? (false, "Không tìm thấy", null) : (true, null, td);
        }

        public async Task<(bool Success, string Error, List<TestDrive> Data)> GetByCustomerAsync(Guid customerId)
        {
            var list = await _repo.GetByCustomerAsync(customerId);
            return (true, null, list);
        }

        public async Task<(bool Success, string Error, TestDrive Data)> ConfirmAsync(Guid id)
        {
            var td = await _repo.GetByIdAsync(id);
            if (td == null) return (false, "Không tìm thấy", null);
            td.Status = TestDriveStatus.Confirmed;
            td.UpdatedAt = DateTime.UtcNow;
            var ok = await _repo.UpdateAsync(td);
            return ok ? (true, null, td) : (false, "Không xác nhận được", null);
        }

        public async Task<(bool Success, string Error, TestDrive Data)> CompleteAsync(Guid id, bool success)
        {
            var td = await _repo.GetByIdAsync(id);
            if (td == null) return (false, "Không tìm thấy", null);
            td.Status = success ? TestDriveStatus.Successfully : TestDriveStatus.Failed;
            td.UpdatedAt = DateTime.UtcNow;
            var ok = await _repo.UpdateAsync(td);
            return ok ? (true, null, td) : (false, "Không cập nhật được", null);
        }

        public async Task<(bool Success, string Error, TestDrive Data)> CancelAsync(Guid id)
        {
            var td = await _repo.GetByIdAsync(id);
            if (td == null) return (false, "Không tìm thấy", null);
            td.Status = TestDriveStatus.Canceled;
            td.UpdatedAt = DateTime.UtcNow;
            var ok = await _repo.UpdateAsync(td);
            return ok ? (true, null, td) : (false, "Không hủy được", null);
        }
    }
}
