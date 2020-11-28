using Xunit;

namespace CSInside.Test
{
    public class CommentListRequestTests
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
        public async void CommentListRequestTest()
        {
            CommentListRequest request = Service.CreateCommentListRequest("programming", 1540251);
            Comment[] comments = await request.ExecuteAsync();
            Assert.True(comments.Length > 1);
        }

        [Fact]
        public async void AdultGalleryCommentListRequestTest()
        {
            CommentListRequest request = Service.CreateCommentListRequest("nude", 122229);
            Comment[] comments = await request.ExecuteAsync();
            Assert.True(comments.Length > 1);
        }

        [Fact]
        public async void RestrictedGalleryCommentListRequestTest()
        {
            CommentListRequest request = Service.CreateCommentListRequest("python", 124);
            Comment[] comments = await request.ExecuteAsync();
            Assert.True(comments.Length > 1);
        }

        [Fact]
        public async void DeletedPostCommentListRequestTest()
        {
            CommentListRequest request = Service.CreateCommentListRequest("programming", 1540253);
            Assert.Null(await request.ExecuteAsync());
        }

        [Fact]
        public async void CommentListRequestWithInvalidAuthTokenTest()
        {
            CommentListRequest request = new ApiService(new AuthTokenProvider("InvalidToken")).CreateCommentListRequest("programming", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ExecuteAsync(); });
            Assert.Contains("올바르지 않은 인증 토큰", exception.Message);
        }
    }
}
