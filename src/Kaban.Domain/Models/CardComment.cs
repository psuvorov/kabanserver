using System;
using Kaban.Domain.Models.Common;

namespace Kaban.Domain.Models
{
    public class CardComment : AuditableEntity, IIsDeleted
    {
        public Guid Id { get; set; }
        
        public string Text { get; set; }
        
        public Guid CardId { get; set; }
        
        public Card Card { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}