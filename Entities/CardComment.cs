using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KabanServer.Entities.Common;

namespace KabanServer.Entities
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