using Xunit;

namespace CSInside.XUnit
{
    public class CommentReaderTests
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
        public async void ReadCommentTest()
        {
            IReader<Comment[]?> reader = Service.CreateCommentReader("programming", 1540251);
            Comment[]? comments = await reader.ReadAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void ReadAdultGalleryCommentTest()
        {
            IReader<Comment[]?> reader = Service.CreateCommentReader("nude", 122229);
            Comment[]? comments = await reader.ReadAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void ReadRestrictedGalleryCommentTest()
        {
            IReader<Comment[]?> reader = Service.CreateCommentReader("python", 124);
            Comment[]? comments = await reader.ReadAsync();
            Assert.True(comments?.Length > 1);
        }

        [Fact]
        public async void ReadDeletedPostCommentTest()
        {
            IReader<Comment[]?> reader = Service.CreateCommentReader("programming", 1540253);
            Assert.Null(await reader.ReadAsync());
        }

        [Fact]
        public async void  ReadWithInvalidAuthTokenTest()
        {
            IReader<Comment[]?> request = new ApiService(new AuthTokenProvider("InvalidToken")).CreateCommentReader("programming", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ReadAsync(); });
            Assert.Contains("올바르지 않은 인증 토큰", exception.Message);
        }
#nullable restore
    }
}
