using System;
using Xunit;

namespace CSInside.Test
{
    public class PostReaderTests
    {
        static ApiService service;
        static ApiService Service 
        {
            get
            {
                if(service == null)
                    service = new ApiService(new AuthTokenProvider());
                return service;
            }
        }
#nullable enable
        [Fact]
        public async void ReadNormalPostTest()
        {
            IReader<Post?> request = Service.CreatePostReader("programming", 1540251);
            Post? post = await request.ReadAsync();
            Assert.True(post != null);
            Assert.True(post?.Title == "Test 1");
            Assert.True(post?.TimeStamp == new DateTime(2020, 11, 28, 11, 45, 00));
        }

        [Fact]
        public async void ReadDeletedPostTest()
        {
            IReader<Post?> request = Service.CreatePostReader("programming", 1540253);
            Post? post = await request.ReadAsync();
            Assert.True(post == null);
        }

        [Fact]
        public async void ReadWithInvalidAuthTokenTest()
        {
            IReader<Post?> request = new ApiService(new AuthTokenProvider("InvalidToken")).CreatePostReader("programming", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ReadAsync(); });
            Assert.Contains("올바르지 않은 인증 토큰", exception.Message);
        }

        [Fact]
        public async void ReadAdultGalleryPostTest()
        {
            IReader<Post?> request = Service.CreatePostReader("nude", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ReadAsync(); });
            Assert.Contains("성인 갤러리", exception.Message);
        }

        [Fact]
        public async void ReadRestrictedGalleryPostTest()
        {
            IReader<Post?> request = Service.CreatePostReader("python", 124);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ReadAsync(); });
            Assert.Contains("접근이 제한된", exception.Message);
        }
#nullable restore
    }
}
