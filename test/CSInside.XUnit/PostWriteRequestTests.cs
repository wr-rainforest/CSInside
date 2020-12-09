using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSInside.XUnit
{
    public class PostWriteRequestTests
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
        public async void PostWriteRequestTest()
        {
            ParagraphCollection paragraphs = new ParagraphCollection();
            paragraphs.Add(new StringParagraph("1문단 첫번째 줄\r\n1문단 두번째 줄"));
            paragraphs.Add(new StringParagraph("2문단 첫번째 줄\r\n2문단 두번째 줄"));
            PostWriteRequest request = Service.CreatePostWriteRequest(
                "programming",
                DateTime.Now.ToString(),
                "ㅇㅇ",
                "password",
                paragraphs);
            int postNo = await request.ExecuteAsync();
            Assert.True(postNo > 0);
            try
            {
                await Service.CreatePostDeleteRequest("programming", postNo, "password").ExecuteAsync();
            }
            catch
            {

            }
        }
    }
}
