using System;

namespace Kaban.API.Controllers.Requests.Lists
{
    public class CopyListRequest
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
    }
}