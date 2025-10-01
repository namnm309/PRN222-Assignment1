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

        public async Task<bool> CreateUserAsync(Users user)
        {
            await _dbContext.Users.AddAsync(user);
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        // Legacy signatures, kept for compatibility
        public bool Login(string username, string password)
        {
            var user = GetByEmailAsync(username).GetAwaiter().GetResult();
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public bool Register(string username, string password, string email)
        {
            var exists = GetByEmailAsync(email).GetAwaiter().GetResult();
            if (exists != null) return false;

            var now = DateTime.UtcNow;
            var user = new Users
            {
                FullName = username,
                Email = email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            };

            return CreateUserAsync(user).GetAwaiter().GetResult();
        }
    }
}
