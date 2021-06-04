using System;

namespace Kaban.API.Dto.CardComments
{
    public class CreateCardCommentDto
    {
        public string Text { get; set; }
        public Guid CardId { get; set; }
    }
}