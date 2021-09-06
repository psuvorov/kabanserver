using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kaban.Domain.Models.Common;

namespace Kaban.Domain.Models
{
    public class Board : AuditableEntity, ICanBeDeleted
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<List> Lists { get; set; } = new Collection<List>();
        
        public bool IsDeleted { get; set; }
    }
}