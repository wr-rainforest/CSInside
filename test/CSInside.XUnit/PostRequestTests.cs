using System;
using Xunit;

namespace CSInside.XUnit
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
            PostRequest request = Service.CreatePostRequest("programming", 1540251);
            Post? post = await request.ExecuteAsync();
            Assert.True(post != null);
            Assert.True(post?.Title == "Test 1");
            Assert.True(post?.TimeStamp == new DateTime(2020, 11, 28, 11, 45, 00));
        }

        [Fact]
        public async void DeletedPostRequestTest()
        {
            PostRequest request = Service.CreatePostRequest("programming", 1540253);
            Post? post = await request.ExecuteAsync();
            Assert.True(post == null);
        }

        [Fact]
        public async void AdultGalleryPostRequestTest()
        {
            PostRequest request = Service.CreatePostRequest("nude", 1540253);
            Post? post = await request.ExecuteAsync();
            Assert.True(post == null);
        }

        [Fact]
        public async void RestrictedGalleryPostRequestTest()
        {
            PostRequest request = Service.CreatePostRequest("python", 124);
            Post? post = await request.ExecuteAsync();
            Assert.True(post == null);
        }
#nullable restore
    }
}
