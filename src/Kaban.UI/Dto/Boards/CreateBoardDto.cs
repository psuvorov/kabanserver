using System.ComponentModel.DataAnnotations;

namespace Kaban.UI.Dto.Boards
{
    public class CreateBoardDto
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}