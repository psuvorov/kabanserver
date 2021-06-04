using System;
using System.Collections.Generic;

namespace Kaban.API.Dto.Boards
{
    public class BoardDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BoardUserDto Author { get; set; }
        public IEnumerable<BoardUserDto> Participants { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}