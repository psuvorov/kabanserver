using System;
using System.Security.Claims;
using Kaban.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kaban.API.Helpers
{
    public class StoredUser : IStoredUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoredUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var claimValue = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return claimValue is null ? Guid.Empty : Guid.Parse(claimValue);
        }
    }
}