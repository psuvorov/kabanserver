using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kaban.UI.Entities.Common;

namespace Kaban.UI.Entities
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