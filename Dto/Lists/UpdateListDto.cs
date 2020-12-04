using System;
using System.ComponentModel.DataAnnotations;

namespace KabanServer.Dto.Lists
{
    public class UpdateListDto
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? OrderNumber { get; set; }
        public bool? IsArchived { get; set; }
    }
}