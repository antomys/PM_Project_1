using System;

namespace PollLibrary.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : 
            base($"Invalid Password, [{message}]!")
        {
            
        }
    }
}