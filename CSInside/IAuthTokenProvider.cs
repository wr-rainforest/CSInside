using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    public interface IAuthTokenProvider
    {
        public string GetAppId();

        public string GetClientToken();
    }
}
