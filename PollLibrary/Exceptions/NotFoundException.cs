using System;

namespace PollLibrary.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            
        }

        public NotFoundException(string message) :
            base($"Not found item {message}")
        {
            
        }
    }
}