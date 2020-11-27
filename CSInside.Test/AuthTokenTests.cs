using System;
using System.Collections.Generic;
using System.Text;
using CSInside;
using Xunit;

namespace CSInside.Test
{
    public class AuthTokenTests
    {
        [Fact]
        public void AuthTokenTest()
        {
            AuthTokenProvider authTokenProvider = new AuthTokenProvider();
            string appId = authTokenProvider.GetAppId();
            Assert.True(authTokenProvider.GetAppId().Length == 60);
            byte[] arr = Convert.FromBase64String(Encoding.ASCII.GetString(Convert.FromBase64String(appId)));
            Assert.True(arr.Length == 32);
        }
    }
}
