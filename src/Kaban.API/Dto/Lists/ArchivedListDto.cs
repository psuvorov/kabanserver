using System;

namespace Kaban.API.Dto.Lists
{
    public class ArchivedListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Archived { get; set; }
    }
}