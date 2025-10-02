using System;
using System.Threading.Tasks;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enum;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAuthen _authRepository;

        public AuthenService(IAuthen authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<(bool Success, string Error, Users User)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Email và mật khẩu là bắt buộc", null);
            }

            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, "Email không tồn tại", null);
            }

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!ok)
            {
                return (false, "Mật khẩu không đúng", null);
            }

            if (!user.IsActive)
            {
                return (false, "Tài khoản đã bị khóa", null);
            }

            return (true, null, user);
        }

    }
}
