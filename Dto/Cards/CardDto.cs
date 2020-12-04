using System;

namespace KabanServer.Dto.Cards
{
    public class CardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public string CoverImagePath { get; set; }
        public CoverImageOrientation CoverImageOrientation { get; set; }
        public Guid ListId { get; set; }
    }

    public enum CoverImageOrientation
    {
        Horizontal,
        Vertical
    }
}