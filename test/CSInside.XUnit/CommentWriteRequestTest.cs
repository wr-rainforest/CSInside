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

        [Fact]
        public async void DCConCommentWriteRequestTest()
        {
            DCCon dccon = new DCCon()
            {
                DetailIndex = "1613979",
                Title = "헐",
                ImageUri = "https://dcimg5.dcinside.com/dccon.php?no=62b5df2be09d3ca567b1c5bc12d46b394aa3b1058c6e4d0ca41648b65ceb246e13df9546348593b9b03553cb2b363e94da0bda2f33af133d69a3e3bd02836bc703d2a6",
                PackageIndex = 63214
            };
            DCConParagraph paragraph = new DCConParagraph(dccon);
            CommentWriteRequest commentWriteRequest = Service.CreateCommentWriteRequest(
                "programming",
                1476608,
                "ㅇㅇ",
                "password",
                paragraph);
            var result = await commentWriteRequest.ExecuteAsync();
            Assert.True(result > 0);
        }
    }
}
