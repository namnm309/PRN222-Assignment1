using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLayer.Services
{
    public interface IAuthenService
    {
        Task<(bool Success, string Error, Users User)> LoginAsync(string email, string password);
        Task<(bool Success, string Error, Users User)> RegisterAsync(string fullName, string email, string password);
    }
}
