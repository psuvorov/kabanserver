using System;
using System.ComponentModel.DataAnnotations;

namespace Kaban.API.Controllers.Requests.Cards
{
    public class UpdateCardRequest
    {
        [Required]
        public Guid CardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? ListId { get; set; } // TODO: wtf??
        public bool? IsArchived { get; set; }

    }
}