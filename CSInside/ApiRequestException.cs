using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    class ApiRequestException : Exception
    {
        public int Code { get; private set; }

        public ApiRequestException(int errorCode, string message) : base(message)
        {
            Code = errorCode;
        }

        public ApiRequestException(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            Code = errorCode;
        }
    }
}
