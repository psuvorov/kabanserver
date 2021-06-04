using System;

namespace Kaban.Domain.Models.Common
{
    public interface ICanBeArchived
    {
        bool IsArchived { get; set; }
        
        DateTime? Archived { get; set; }
    }
}