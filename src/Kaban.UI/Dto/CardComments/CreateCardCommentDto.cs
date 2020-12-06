using System;

namespace Kaban.UI.Dto.CardComments
{
    public class CreateCardCommentDto
    {
        public string Text { get; set; }
        public Guid CardId { get; set; }
    }
}