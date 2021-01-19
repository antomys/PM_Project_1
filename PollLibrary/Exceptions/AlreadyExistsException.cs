using System;

namespace PollLibrary.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message) :
            base($"This Account {message} already exists! \nConsider Logging in\n")
        {
            
        }
    }
}