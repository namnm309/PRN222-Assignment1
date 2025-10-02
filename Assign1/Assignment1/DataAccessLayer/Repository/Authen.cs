using System;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class Authen : IAuthen
    {
        private readonly AppDbContext _dbContext;

        public Authen(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Users> GetByEmailAsync(string email)
        {
            var normalized = email?.Trim().ToLower();
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        // Legacy signatures, kept for compatibility
        public bool Login(string username, string password)
        {
            var user = GetByEmailAsync(username).GetAwaiter().GetResult();
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
