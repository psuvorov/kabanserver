using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.UI.Dto.Cards
{
    public class RenumberCardDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public int OrderNumber { get; set; }
    }
}