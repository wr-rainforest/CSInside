using Xunit;

namespace CSInside.XUnit
{
    public class CommentRequestTests
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
#nullable enable
        [Fact]
        public async void CommentRequestTest()
        {
            CommentRequest request = Service.CreateCommentRequest("programming", 1540251);
            Comment[]? comments = await request.ExecuteAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void AdultGalleryCommentRequestTest()
        {
            CommentRequest request = Service.CreateCommentRequest("nude", 122229);
            Comment[]? comments = await request.ExecuteAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void RestrictedGalleryCommentRequestTest()
        {
            CommentRequest request = Service.CreateCommentRequest("python", 124);
            Comment[]? comments = await request.ExecuteAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void DeletedPostCommentRequestTest()
        {
            CommentRequest request = Service.CreateCommentRequest("programming", 1540253);
            Assert.Null(await request.ExecuteAsync());
        }

        [Fact]
        public async void CommentRequestWithInvalidAuthTokenTest()
        {
            CommentRequest request = new ApiService(new AuthTokenProvider("InvalidToken")).CreateCommentRequest("programming", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ExecuteAsync(); });
            Assert.Contains("올바르지 않은 인증 토큰", exception.Message);
        }
#nullable restore
    }
}
