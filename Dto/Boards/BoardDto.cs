using System;
using System.Collections.Generic;
using KabanServer.Dto.Lists;

namespace KabanServer.Dto.Boards
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