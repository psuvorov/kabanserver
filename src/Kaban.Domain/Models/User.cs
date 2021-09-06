using System;
using Kaban.Domain.Models.Common;

namespace Kaban.Domain.Models
{
    public class User: ICanBeDeleted
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public byte[] PasswordHash { get; set; }
        
        public byte[] PasswordSalt { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}