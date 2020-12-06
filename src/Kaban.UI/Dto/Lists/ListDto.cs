using System;
using System.Collections.Generic;
using Kaban.UI.Dto.Cards;

namespace Kaban.UI.Dto.Lists
{
    public class ListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public IEnumerable<CardDto> Cards { get; set; }
    }
}