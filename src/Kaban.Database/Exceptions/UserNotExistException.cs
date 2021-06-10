using System;

namespace Kaban.Database.Exceptions
{
    public class UserNotExistException : Exception
    {
        public UserNotExistException(string message) : base(message)
        {
        }
    }
}