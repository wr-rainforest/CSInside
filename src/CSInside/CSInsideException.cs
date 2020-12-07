using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    public class CSInsideException : Exception
    {
        public CSInsideException(string message) : base(message)
        {

        }

        public CSInsideException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
