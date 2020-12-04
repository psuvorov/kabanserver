using System;
using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Cards
{
    public class CreateCardDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        [Required]
        public Guid ListId { get; set; }
        
    }
}