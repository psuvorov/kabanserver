using System;

namespace Kaban.API.Controllers.Requests.CardComments
{
    public class CreateCardCommentRequest
    {
        public string Text { get; set; }
        public Guid CardId { get; set; }
    }
}