using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KabanServer.Entities.Common;

namespace KabanServer.Entities
{
    public class Card : AuditableEntity, IIsDeleted, ICanBeArchived
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int OrderNumber { get; set; }
        
        public Guid ListId { get; set; }

        public List List { get; set; }

        public ICollection<CardComment> CardComments { get; set; } = new Collection<CardComment>();

        public bool IsArchived { get; set; }
        
        public DateTime? Archived { get; set; }

        public bool IsDeleted { get; set; }
    }
}