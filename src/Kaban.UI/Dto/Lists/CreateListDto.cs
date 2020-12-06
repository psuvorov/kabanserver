using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.UI.Dto.Lists
{
    public class CreateListDto
    {
        [Required]
        public string Name { get; set; }
        
        public int OrderNumber { get; set; }
        
        [Required]
        public Guid BoardId { get; set; }
        
    }
}