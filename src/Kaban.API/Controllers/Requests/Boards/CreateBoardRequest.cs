using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Boards
{
    public class CreateBoardRequest
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}