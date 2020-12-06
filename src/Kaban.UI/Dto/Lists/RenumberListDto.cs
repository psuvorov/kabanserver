using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.UI.Dto.Lists
{
    public class RenumberListDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int OrderNumber { get; set; }
    }
}