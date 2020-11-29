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
        public void AccessTokenTest()
        {
            AuthTokenProvider authTokenProvider = new AuthTokenProvider();
            string appId = authTokenProvider.GetAccessToken();
            Assert.True(authTokenProvider.GetAccessToken().Length == 60);
            byte[] arr = Convert.FromBase64String(Encoding.ASCII.GetString(Convert.FromBase64String(appId)));
            Assert.True(arr.Length == 32);
        }
    }
}
