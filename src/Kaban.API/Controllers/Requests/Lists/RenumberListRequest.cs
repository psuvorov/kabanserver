using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Lists
{
    public class RenumberListRequest
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int OrderNumber { get; set; }
    }
}