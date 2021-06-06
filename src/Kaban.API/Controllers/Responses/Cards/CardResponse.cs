using System;

namespace Kaban.API.Controllers.Responses.Cards
{
    public class CardResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public string CoverImagePath { get; set; }
        public CoverImageOrientationDto CoverImageOrientation { get; set; }
        public Guid ListId { get; set; }
    }

    public enum CoverImageOrientationDto
    {
        Horizontal,
        Vertical
    }
}