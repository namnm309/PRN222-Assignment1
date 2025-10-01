using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IAuthen
    {
        //Login 
        public bool Login(string username, string password);
        //Register
        public bool Register(string username, string password, string email);

        // New async APIs used by BusinessLayer
        Task<Entities.Users> GetByEmailAsync(string email);
        Task<bool> CreateUserAsync(Entities.Users user);
    }
}
