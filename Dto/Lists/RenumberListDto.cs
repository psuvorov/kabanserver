using System;
using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Lists
{
    public class RenumberListDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int OrderNumber { get; set; }
    }
}