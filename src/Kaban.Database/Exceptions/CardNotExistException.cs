using System;

namespace Kaban.Database.Exceptions
{
    public class CardNotExistException : Exception
    {
        public CardNotExistException(string message) : base(message)
        {
        }
    }
}