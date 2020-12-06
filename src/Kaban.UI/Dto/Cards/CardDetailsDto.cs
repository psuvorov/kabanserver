using System;
using System.Collections.Generic;
using Kaban.UI.Dto.CardComments;
using Kaban.UI.Entities;

namespace Kaban.UI.Dto.Cards
{
    public class CardDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public Guid ListId { get; set; }
        public string ListName { get; set; }
        
        // TODO: Pass here only first 10 comments. Further comments should be acquired via another endpoint (for pagination / virtual scrolling purposes)
        public IEnumerable<CardCommentDto> Comments { get; set; } 
    }
}