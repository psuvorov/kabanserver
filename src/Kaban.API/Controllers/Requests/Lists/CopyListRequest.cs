using System;

namespace Kaban.API.Controllers.Requests.Lists
{
    public class CopyListRequest
    {
        public Guid BoardId { get; set; }
        public Guid ListId { get; set; }
    }
}