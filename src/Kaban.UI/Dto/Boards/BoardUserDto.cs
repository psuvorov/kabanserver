using System;

namespace Kaban.UI.Dto.Boards
{
    public class BoardUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserPicture { get; set; }
    }
}