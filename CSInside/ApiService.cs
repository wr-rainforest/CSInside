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

        public PostRequest CreatePostRequest(string galleryId, int postNo)
        {
            return new PostRequest(galleryId, postNo, client, authTokenProvider);
        }

        public CommentListRequest CreateCommentListRequest(string galleryId, int postNo)
        {
            return new CommentListRequest(galleryId, postNo, client, authTokenProvider);
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
