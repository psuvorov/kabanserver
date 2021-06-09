using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Boards
{
    public class UpdateBoardRequest
    {
        [Required]
        public Guid BoardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}