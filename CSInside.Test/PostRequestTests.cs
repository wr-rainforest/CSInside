using System;
using Xunit;

namespace CSInside.Test
{
    public class PostRequestTests
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
        public async void PostRequestTest()
        {
            IRequest<Post?> request = Service.CreatePostRequest("programming", 1540251);
            Post? post = await request.ExecuteAsync();
            Assert.True(post != null);
            Assert.True(post?.Title == "Test 1");
            Assert.True(post?.TimeStamp == new DateTime(2020, 11, 28, 11, 45, 00));
        }

        [Fact]
        public async void DeletedPostRequestTest()
        {
            IRequest<Post?> request = Service.CreatePostRequest("programming", 1540253);
            Post? post = await request.ExecuteAsync();
            Assert.True(post == null);
        }

        [Fact]
        public async void PostRequestWithInvalidAuthTokenTest()
        {
            IRequest<Post?> request = new ApiService(new AuthTokenProvider("InvalidToken")).CreatePostRequest("programming", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ExecuteAsync(); });
            Assert.Contains("올바르지 않은 인증 토큰", exception.Message);
        }

        [Fact]
        public async void AdultGalleryPostRequestTest()
        {
            IRequest<Post?> request = Service.CreatePostRequest("nude", 1540253);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ExecuteAsync(); });
            Assert.Contains("성인 갤러리", exception.Message);
        }

        [Fact]
        public async void RestrictedGalleryPostRequestTest()
        {
            IRequest<Post?> request = Service.CreatePostRequest("python", 124);
            var exception = await Assert.ThrowsAsync<CSInsideException>(async () => { await request.ExecuteAsync(); });
            Assert.Contains("접근이 제한된", exception.Message);
        }
#nullable restore
    }
}
