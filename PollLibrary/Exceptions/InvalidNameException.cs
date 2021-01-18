using System;

namespace PollLibrary.Exceptions
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : 
            base($"Invalid Name, [{message}]!")
        {
            
        }
    }
}