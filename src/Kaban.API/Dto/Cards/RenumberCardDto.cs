using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Dto.Cards
{
    public class RenumberCardDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public int OrderNumber { get; set; }
    }
}