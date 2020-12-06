using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kaban.UI.Entities.Common;

namespace Kaban.UI.Entities
{
    public class Board : AuditableEntity, IIsDeleted
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<List> Lists { get; set; } = new Collection<List>();
        
        public bool IsDeleted { get; set; }
    }
}