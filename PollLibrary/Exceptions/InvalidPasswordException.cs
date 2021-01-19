using System;

namespace PollLibrary.Exceptions
{
    public class EmptyPollException : Exception
    {
        public EmptyPollException(string message) : 
            base($"This Poll {message} has no questions!")
        {
            
        }
    }
}