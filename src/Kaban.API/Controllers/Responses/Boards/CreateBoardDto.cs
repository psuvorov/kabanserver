using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Responses.Boards
{
    public class CreateBoardDto
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}