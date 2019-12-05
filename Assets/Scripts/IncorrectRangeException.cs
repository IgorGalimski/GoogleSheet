using System;

namespace DefaultNamespace
{
    public class IncorrectRangeException : Exception
    {
        public IncorrectRangeException(string message) : base(message)
        {
        }
    }
}