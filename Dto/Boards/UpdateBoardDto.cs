using System;
using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Boards
{
    public class UpdateBoardDto
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}