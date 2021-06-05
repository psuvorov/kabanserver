using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kaban.Domain.Models;

namespace Kaban.Domain.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        
        User Get(Guid id);

        User Get(string email);
        
        Task<User> Create(User user, string password);

        Task<User> Authenticate(string email, string password);

        // User GetCurrentUser();

        void Logout();
        
        void Update(User user, string password = null);
        
        void Delete(Guid id);
    }
}