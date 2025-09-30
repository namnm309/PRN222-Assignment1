using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
	public interface IUserService
	{
		Task<Users> RegisterAsync(string userName, string fullName, string email, string password, string? phoneNumber, string? address, CancellationToken cancellationToken = default);
		Task<Users?> AuthenticateAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default);
		Task<bool> IsUserNameTakenAsync(string userName, CancellationToken cancellationToken = default);
		Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
	}
}

