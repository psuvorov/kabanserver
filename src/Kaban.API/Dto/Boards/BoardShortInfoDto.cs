using System;

namespace Kaban.API.Dto.Boards
{
    public class BoardShortInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WallpaperPreviewPath { get; set; }
    }
}