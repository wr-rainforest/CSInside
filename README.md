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
            Post post = await postRequest.Execute();

            CommentListRequest commentListRequest = service.CreateCommentListRequest("programming", 1476608);
            Comment[] comments = await commentListRequest.Execute();
             
            ImageRequest imageRequest = service.CreateImageRequest("https://dcimg6.dcinside.com/....3ca1e");
            byte[] previewImage = await imageRequest.Execute(ImageType.Preview);
            byte[] webImage = await imageRequest.Execute(ImageType.Web);
            byte[] originalImage = await imageRequest.Execute(ImageType.Origin);
        }
    }
}
```
## ������ ���̺귯��
- [KotlinInside](https://github.com/organization/KotlinInside)
- [goinside](https://github.com/geeksbaek/goinside)
- [pyinside](https://github.com/jeongsj/pyinside)
