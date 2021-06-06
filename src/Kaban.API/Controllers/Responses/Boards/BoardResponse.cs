using System;
using System.Collections.Generic;
using Kaban.API.Controllers.Responses.Lists;

namespace Kaban.API.Controllers.Responses.Boards
{
    public class BoardResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WallpaperPath { get; set; }
        public IEnumerable<ListResponse> Lists { get; set; }
    }
}