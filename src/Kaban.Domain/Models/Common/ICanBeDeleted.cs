namespace Kaban.Domain.Models.Common
{
    public interface ICanBeDeleted
    {
        bool IsDeleted { get; set; }
    }
}