using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class DealerRepository : IDealerRepository
    {
        private readonly AppDbContext _context;

        public DealerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dealer>> GetAllAsync()
        {
            return await _context.Dealer.ToListAsync();
        }

        public async Task<Dealer> GetByIdAsync(Guid id)
        {
            return await _context.Dealer.FindAsync(id);
        }

        public async Task AddAsync(Dealer dealer)
        {
            _context.Dealer.Add(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Dealer dealer)
        {
            _context.Dealer.Update(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var dealer = await _context.Dealer.FindAsync(id);
            if (dealer != null)
            {
                _context.Dealer.Remove(dealer);
                await _context.SaveChangesAsync();
            }
        }
    }

}
