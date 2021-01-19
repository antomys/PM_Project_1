using System;

namespace PollLibrary.Exceptions
{
    public class PollNotFoundException : Exception
    {
        public PollNotFoundException(string message) :
            base($"Poll with index {message} is not found!!")
        {
            
        }
    }
}