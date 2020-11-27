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
        internal HttpClient Client { get; }

        internal IAuthTokenProvider AuthTokenProvider { get; }

        public ApiService(IAuthTokenProvider authTokenProvider)
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            Client = new HttpClient(handler);
            Client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            AuthTokenProvider = authTokenProvider;
        }

        public ApiService(IAuthTokenProvider authTokenProvider, IWebProxy webProxy)
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip,
                Proxy = webProxy
            };
            Client = new HttpClient(handler);
            Client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            AuthTokenProvider = authTokenProvider;
        }

        public PostRequest CreatePostRequest(string galleryId, int postNo)
        {
            return new PostRequest(galleryId, postNo, this);
        }

        public CommentListRequest CreateCommentListRequest(string galleryId, int postNo)
        {
            return new CommentListRequest(galleryId, postNo, this);
        }

        public ImageRequest CreateImageRequest(string imageUri)
        {
            return new ImageRequest(imageUri, this);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
