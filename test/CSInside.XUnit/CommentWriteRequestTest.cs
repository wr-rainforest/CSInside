using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSInside.XUnit
{
    public class CommentWriteRequestTests
    {
        static ApiService _service;
        static ApiService service
        {
            get
            {
                if (_service == null)
                {
                    AuthTokenProvider authTokenProvider = new AuthTokenProvider();
                    authTokenProvider.GetAccessToken();
                    Task.Delay(5000).Wait();
                    _service = new ApiService(authTokenProvider);
                }
                return _service;
            }
        }

        [Fact]
        public async void CommentWriteRequestTest()
        {
            CommentWriteRequest commentWriteRequest = service.CreateCommentWriteRequest(
                "programming",
                1476608,
                "ㅇㅇ",
                "password",
                new StringParagraph(DateTime.Now.ToString()));
            var commentNo = await commentWriteRequest.ExecuteAsync();
            Assert.True(commentNo > 0);

            try
            {
                var delRequest = service.CreateCommentDeleteRequest();
                delRequest.Params.GalleryId = "programming";
                delRequest.Params.PostNo = 1476608;
                delRequest.Params.CommentNo = commentNo;
                delRequest.Params.Password = "password";
                await delRequest.ExecuteAsync();
            }
            catch
            {

            }
        }

        [Fact]
        public async void DCConCommentWriteRequestTest()
        {
            DCCon dccon = new DCCon()
            {
                DetailIndex = 1613979,
                Title = "헐",
                ImageUri = "https://dcimg5.dcinside.com/dccon.php?no=62b5df2be09d3ca567b1c5bc12d46b394aa3b1058c6e4d0ca41648b65ceb246e13df9546348593b9b03553cb2b363e94da0bda2f33af133d69a3e3bd02836bc703d2a6",
                PackageIndex = 63214
            };
            DCConParagraph paragraph = new DCConParagraph(dccon);
            CommentWriteRequest commentWriteRequest = service.CreateCommentWriteRequest(
                "programming",
                1476608,
                "ㅇㅇ",
                "password",
                paragraph);
            var commentNo = await commentWriteRequest.ExecuteAsync();
            Assert.True(commentNo > 0);
            try
            {
                var delRequest = service.CreateCommentDeleteRequest();
                delRequest.Params.GalleryId = "programming";
                delRequest.Params.PostNo = 1476608;
                delRequest.Params.CommentNo = commentNo;
                delRequest.Params.Password = "password";
                await delRequest.ExecuteAsync();
            }
            catch
            {

            }
        }
    }
}
