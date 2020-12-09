using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSInside.XUnit
{
    public class CommentWriteRequestTests
    {
        static ApiService service;
        static ApiService Service
        {
            get
            {
                if (service == null)
                {
                    AuthTokenProvider authTokenProvider = new AuthTokenProvider();
                    authTokenProvider.GetAccessToken();
                    Task.Delay(5000).Wait();
                    service = new ApiService(authTokenProvider);
                }
                return service;
            }
        }

        [Fact]
        public async void CommentWriteRequestTest()
        {
            CommentWriteRequest commentWriteRequest = Service.CreateCommentWriteRequest(
                "programming",
                1476608,
                "ㅇㅇ",
                "password",
                new StringParagraph(DateTime.Now.ToString()));
            var result = await commentWriteRequest.ExecuteAsync();
            Assert.True(result > 0);

        }
    }
}
