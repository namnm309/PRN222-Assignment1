using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace BusinessLayer.Services
{
	public class UserService : IUserService
	{
		private readonly IRepository<Users> _userRepository;

		public UserService(IRepository<Users> userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<Users> RegisterAsync(string userName, string fullName, string email, string password, string? phoneNumber, string? address, CancellationToken cancellationToken = default)
		{
			// Kiểm tra tồn tại
			if (await IsUserNameTakenAsync(userName, cancellationToken))
				throw new InvalidOperationException("Username already taken");
			if (await IsEmailTakenAsync(email, cancellationToken))
				throw new InvalidOperationException("Email already registered");

			var user = new Users
			{
				UserName = userName,
				FullName = fullName,
				Email = email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
				PhoneNumber = phoneNumber ?? string.Empty,
				Address = address ?? string.Empty,
				Role = DataAccessLayer.Enum.UserRole.DealerStaff,
				IsActive = true
			};

			return await _userRepository.AddAsync(user, cancellationToken);
		}

        public async Task<Users?> AuthenticateAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default)
		{
            var users = await _userRepository.ListAsync(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail,
                cancellationToken);
            var user = users.FirstOrDefault();
			if (user == null) return null;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
		}

		public async Task<bool> IsUserNameTakenAsync(string userName, CancellationToken cancellationToken = default)
		{
            var count = await _userRepository.CountAsync(u => u.UserName == userName, cancellationToken);
			return count > 0;
		}

		public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
		{
            var count = await _userRepository.CountAsync(u => u.Email == email, cancellationToken);
			return count > 0;
		}
	}
}
