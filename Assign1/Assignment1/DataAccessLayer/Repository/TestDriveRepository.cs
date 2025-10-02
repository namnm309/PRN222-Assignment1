using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class TestDriveRepository : ITestDriveRepository
    {
        private readonly AppDbContext _db;
        public TestDriveRepository(AppDbContext db) => _db = db;

        public Task<TestDrive?> GetByIdAsync(Guid id)
            => _db.TestDrive.Include(t => t.Product).Include(t => t.Customer).Include(t => t.Dealer)
                            .FirstOrDefaultAsync(t => t.Id == id);

        public Task<List<TestDrive>> GetByCustomerAsync(Guid customerId)
            => _db.TestDrive.Where(t => t.CustomerId == customerId)
                            .OrderByDescending(t => t.ScheduledDate)
                            .ToListAsync();

        public Task<List<TestDrive>> GetByDealerAndProductInRangeAsync(Guid dealerId, Guid productId, DateTime from, DateTime to)
            => _db.TestDrive.Where(t => t.DealerId == dealerId && t.ProductId == productId
                                        && t.ScheduledDate < to && t.ScheduledDate.AddMinutes(90) > from
                                        && t.Status != Enum.TestDriveStatus.Canceled)
                            .ToListAsync();

        public async Task<bool> CreateAsync(TestDrive td)
        {
            await _db.TestDrive.AddAsync(td);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(TestDrive td)
        {
            _db.TestDrive.Update(td);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
