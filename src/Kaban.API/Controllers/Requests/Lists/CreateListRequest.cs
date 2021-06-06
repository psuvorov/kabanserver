using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Lists
{
    public class CreateListRequest
    {
        [Required]
        public string Name { get; set; }
        
        public int OrderNumber { get; set; }
        
        [Required]
        public Guid BoardId { get; set; }
        
    }
}