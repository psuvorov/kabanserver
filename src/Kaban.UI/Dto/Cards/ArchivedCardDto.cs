using System;

namespace Kaban.UI.Dto.Cards
{
    public class ArchivedCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ListName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Archived { get; set; }
    }
}