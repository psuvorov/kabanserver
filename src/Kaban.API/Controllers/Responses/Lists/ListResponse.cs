using System;
using System.Collections.Generic;
using Kaban.API.Controllers.Responses.Cards;

namespace Kaban.API.Controllers.Responses.Lists
{
    public class ListResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public IEnumerable<CardResponse> Cards { get; set; }
    }
}