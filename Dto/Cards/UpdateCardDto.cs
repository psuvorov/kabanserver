using System;
using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Cards
{
    public class UpdateCardDto
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? ListId { get; set; }
        public bool? IsArchived { get; set; }

    }
}