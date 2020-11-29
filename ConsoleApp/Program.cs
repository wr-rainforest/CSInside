using System.Threading.Tasks;
using CSInside;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AuthTokenProvider authTokenProvider = new AuthTokenProvider();
            //await authTokenProvider.LoginAsync("user_id", "password");

            ApiService service = new ApiService(authTokenProvider);
            //ApiService service = new ApiService(authTokenProvider, new WebProxy("http://127.0.0.1", 9150));

            PostRequest postRequest = service.CreatePostRequest("programming", 1476608);
            Post post = await postRequest.ExecuteAsync();
            
            CommentListRequest commentListRequest = service.CreateCommentListRequest("programming", 1476608);
            Comment[] comments = await commentListRequest.ExecuteAsync();

            ImageRequest imageRequest = service.CreateImageRequest("https://dcimg8.dcinside.co.kr/...");
            byte[] previewImage = await imageRequest.ExecuteAsync(ImageType.Preview);
            byte[] webImage = await imageRequest.ExecuteAsync(ImageType.Web);
            byte[] originalImage = await imageRequest.ExecuteAsync(ImageType.Origin);

            UpvoteRequest upvoteRequest = service.CreateUpvoteRequest("programming", 1476608);
            bool upvoteResult = await upvoteRequest.ExecuteAsync();

            DownvoteRequest downvoteRequest = service.CreateDownvoteRequest("programming", 1476608);
            bool downvoteResult = await downvoteRequest.ExecuteAsync();
        }
    }
}
