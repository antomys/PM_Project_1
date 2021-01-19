using System;
using System.IO;

namespace PollLibrary.Exceptions
{
    public class NotFoundException : FileNotFoundException
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