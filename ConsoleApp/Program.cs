using System;
using System.Net;
using System.Threading.Tasks;
using CSInside;
using Newtonsoft.Json;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IAuthTokenProvider authTokenProvider = new AuthTokenProvider();
            ApiService service = new ApiService(authTokenProvider);

            ArticleRequest articleRequest = service.CreateArticleRequest("programming", 1539207);
            Article article = await articleRequest.Execute();
            Console.WriteLine(JsonConvert.SerializeObject(article, Formatting.Indented));

            CommentListRequest commentListRequest = service.CreateCommentListRequest("programming", 1539207);
            Comment[] comments = await commentListRequest.Execute();
            Console.WriteLine(JsonConvert.SerializeObject(comments, Formatting.Indented));

            ImageRequest imageRequest = service.CreateImageRequest("https://dcimg6.dcinside.com/viewimage.php?&no=24b0d......3ca1e");
            byte[] previewImage = await imageRequest.Execute(ImageType.Preview);
            byte[] webImage = await imageRequest.Execute(ImageType.Web);
            byte[] originalImage = await imageRequest.Execute(ImageType.Origin);
        }
    }
}
