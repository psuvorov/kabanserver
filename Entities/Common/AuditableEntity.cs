using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KabanServer.Entities.Common
{
    public class AuditableEntity
    {
        public User CreatedBy { get; set; }
        
        public DateTime Created { get; set; }
        
        public User LastModifiedBy { get; set; }
        
        public DateTime? LastModified { get; set; }
    }
}