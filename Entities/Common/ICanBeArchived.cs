using System;

namespace KabanServer.Entities.Common
{
    public interface ICanBeArchived
    {
        bool IsArchived { get; set; }
        
        DateTime? Archived { get; set; }
    }
}