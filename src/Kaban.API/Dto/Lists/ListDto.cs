using System;
using System.Collections.Generic;
using Kaban.API.Dto.Cards;

namespace Kaban.API.Dto.Lists
{
    public class ListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public IEnumerable<CardDto> Cards { get; set; }
    }
}