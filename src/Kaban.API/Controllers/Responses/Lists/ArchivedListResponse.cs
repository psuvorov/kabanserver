using System;

namespace Kaban.API.Controllers.Responses.Lists
{
    public class ArchivedListResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Archived { get; set; }
    }
}