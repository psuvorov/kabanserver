using System;

namespace Kaban.Database.Exceptions
{
    public class CardCommentNotExistException : Exception
    {
        public CardCommentNotExistException(string message) : base(message)
        {
        }
    }
}