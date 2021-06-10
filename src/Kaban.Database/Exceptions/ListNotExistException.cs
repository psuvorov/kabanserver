using System;

namespace Kaban.Database.Exceptions
{
    public class ListNotExistException : Exception
    {
        public ListNotExistException(string message) : base(message)
        {
        }
    }
}