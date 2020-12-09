using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CSInside.XUnit
{
    public class PostListRequestTests
    {
        static ApiService service;
        static ApiService Service
        {
            get
            {
                if (service == null)
                    service = new ApiService(new AuthTokenProvider());
                return service;
            }
        }

        [Fact]
        public async void PostListRequestTest()
        {
            var request = Service.CreatePostListRequest("programming", 1);
            var postHeaders = await request.ExecuteAsync();
            Assert.True(postHeaders.Length > 1);
        }
    }
}
