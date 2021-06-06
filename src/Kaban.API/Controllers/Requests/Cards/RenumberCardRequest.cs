using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Cards
{
    public class RenumberCardRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public int OrderNumber { get; set; }
    }
}