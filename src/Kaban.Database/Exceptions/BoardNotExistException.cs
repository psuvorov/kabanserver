using System;

namespace Kaban.Database.Exceptions
{
    public class BoardNotExistException : Exception
    {
        public BoardNotExistException(string message) : base(message)
        {
            
        }
    }
}