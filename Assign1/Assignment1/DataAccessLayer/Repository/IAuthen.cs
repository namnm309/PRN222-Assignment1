using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IAuthen
    {
        public bool Login(string username, string password);

        Task<Entities.Users> GetByEmailAsync(string email);
        Task<bool> CreateAsync(Entities.Users user);
    }
}
