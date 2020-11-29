## CSInside
����λ��̵� ����� API(app, json2)�� C# ���� ���̺귯���Դϴ�.  
## QuickStart
```csharp
using System.Threading.Tasks;
using CSInside;
namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IAuthTokenProvider authTokenProvider = new AuthTokenProvider();
            ApiService service = new ApiService(authTokenProvider);

            PostRequest postRequest = service.CreatePostRequest("programming", 1476608);
            Post post = await postRequest.ExecuteAsync();

            PostDeleteRequest postDeleteRequest = service.CreatePostDeleteRequest("programming", 1476608);
            await postDeleteRequest.ExecuteAsync();

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
```
## ������ ���̺귯��
- [KotlinInside](https://github.com/organization/KotlinInside)
- [goinside](https://github.com/geeksbaek/goinside)
- [pyinside](https://github.com/jeongsj/pyinside)
