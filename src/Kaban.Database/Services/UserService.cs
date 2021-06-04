using System;
using System.Collections.Generic;
using System.Linq;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaban.Database.Services
{
    // TODO: separate into two services

    public class UserService : IUserService
    {
        private readonly DataContext _context;
        
        public UserService(DataContext context)
        {
            _context = context;
        }
        
        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }
        
        public User Get(Guid id)
        {
            return _context.Users.Find(id);
        }

        public User Get(string email)
        {
            return _context.Users.SingleOrDefault(user => user.Email == email);
        }

        public User Create(User user, string password)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));
            if (_context.Users.IgnoreQueryFilters().Any(u => u.Username == user.Username))
                throw new Exception($"User with username '{user.Username}' already exists.");
            if (_context.Users.IgnoreQueryFilters().Any(u => u.Email == user.Email))
                throw new Exception($"Email '{user.Email}' is already in use.");
            if (password is null)
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace only string.", nameof(password));

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            _context.Users.Add(user);
            _context.SaveChanges();
            
            return user;
        }
        
        public User Authenticate(string email, string password)
        {
            if (email is null)
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty or whitespace only string.", nameof(email));
            if (password is null)
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace only string.", nameof(password));
            
            var storedUser = _context.Users.SingleOrDefault(x => x.Email == email);
            
            // check if user with this email exists
            if (storedUser is null)
                return null;
            
            // check if password is correct
            if (!VerifyPasswordHash(password, storedUser.PasswordHash, storedUser.PasswordSalt))
                return null;

            // authentication successful
            return storedUser;
        }
        
        // public User GetCurrentUser()
        // {
        //     var userIdRaw = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     var userId = Guid.Parse(userIdRaw);
        //     
        //     var storedUser = _context.Users.Find(userId);
        //
        //     return storedUser;
        // }
        
        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void Update(User user, string password = null)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));
            if (_context.Users.SingleOrDefault(x => x.Id == user.Id) is null)
                throw new Exception("User not found.");
            
            if (_context.Users.IgnoreQueryFilters().Any(u => u.Username == user.Username))
                throw new Exception($"User with username '{user.Username}' already exists.");
            if (_context.Users.IgnoreQueryFilters().Any(u => u.Email == user.Email))
                throw new Exception($"Email '{user.Email}' is already in use.");
            
            // Update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user is null)
                return;

            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace only string.", nameof(password));

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Param value cannot be empty or whitespace only string.", nameof(password));
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }
    }

    
}