using System;

namespace Kaban.UI.Entities.Common
{
    public interface ICanBeArchived
    {
        bool IsArchived { get; set; }
        
        DateTime? Archived { get; set; }
    }
}