using System.Threading.Tasks;
using CSInside;
#nullable enable
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

            IRequest<Post?> postRequest = service.CreatePostRequest("programming", 1476608);
            Post? post = await postRequest.ExecuteAsync();
            
            IRequest<Comment[]?> commentListRequest = service.CreateCommentListRequest("programming", 1476608);
            Comment[]? comments = await commentListRequest.ExecuteAsync();

            IRequest<bool> upvoteRequest = service.CreateUpvoteRequest("programming", 1476608);
            bool upvoteResult = await upvoteRequest.ExecuteAsync();

            IRequest<bool> downvoteRequest = service.CreateDownvoteRequest("programming", 1476608);
            bool downvoteResult = await downvoteRequest.ExecuteAsync();

            IRequest<PostHeader[]> postHeaderRequest = service.CreatePostSearchRequest("programming", "국비", SearchType.All);
        }
    }
}
