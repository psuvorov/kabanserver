using System;
using System.Collections.Generic;
using Kaban.API.Dto.Lists;

namespace Kaban.API.Dto.Boards
{
    public class BoardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WallpaperPath { get; set; }
        public IEnumerable<ListDto> Lists { get; set; }
    }
}