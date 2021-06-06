using System;
using System.Collections.Generic;

namespace Kaban.API.Controllers.Responses.Boards
{
    public class BoardDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BoardUserResponse Author { get; set; }
        public IEnumerable<BoardUserResponse> Participants { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}