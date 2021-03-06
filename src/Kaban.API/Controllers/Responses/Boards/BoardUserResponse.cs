using System;

namespace Kaban.API.Controllers.Responses.Boards
{
    public class BoardUserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserPicture { get; set; }
    }
}