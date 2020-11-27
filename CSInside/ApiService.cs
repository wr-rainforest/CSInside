using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    public class ApiService : IDisposable
    {
        private readonly HttpClient client;

        private IAuthTokenProvider authTokenProvider;

        public ApiService(IAuthTokenProvider authTokenProvider)
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            this.authTokenProvider = authTokenProvider;
        }

        public ApiService(IAuthTokenProvider authTokenProvider, IWebProxy webProxy)
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip,
                Proxy = webProxy
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            this.authTokenProvider = authTokenProvider;
        }

        public ArticleRequest CreateArticleRequest(string galleryId, int articleNo)
        {
            return new ArticleRequest(galleryId, articleNo, client, authTokenProvider);
        }

        public CommentListRequest CreateCommentListRequest(string galleryId, int articleNo)
        {
            return new CommentListRequest(galleryId, articleNo, client, authTokenProvider);
        }

        public ImageRequest CreateImageRequest(string imageUri)
        {
            return new ImageRequest(imageUri, client, authTokenProvider);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
