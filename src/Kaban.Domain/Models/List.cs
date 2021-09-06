using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kaban.Domain.Models.Common;

namespace Kaban.Domain.Models
{
    public class List : AuditableEntity, ICanBeDeleted, ICanBeArchived
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public int OrderNumber { get; set; }
        
        public Guid BoardId { get; set; }

        public Board Board { get; set; }

        public ICollection<Card> Cards { get; set; } = new Collection<Card>();
        
        public bool IsArchived { get; set; }
        
        public DateTime? Archived { get; set; }

        public bool IsDeleted { get; set; }
    }
}