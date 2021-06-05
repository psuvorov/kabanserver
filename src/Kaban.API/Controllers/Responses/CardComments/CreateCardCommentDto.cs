using System;

namespace Kaban.API.Controllers.Responses.CardComments
{
    public class CreateCardCommentDto
    {
        public string Text { get; set; }
        public Guid CardId { get; set; }
    }
}