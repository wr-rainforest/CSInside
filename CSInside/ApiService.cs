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
            Client.DefaultRequestHeaders.Add("Referer", "http://www.dcinside.com");
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
            Client.DefaultRequestHeaders.Add("Referer", "http://www.dcinside.com");
            AuthTokenProvider = authTokenProvider;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public PostRequest CreatePostRequest(string galleryId, int postNo)
        {
            return new PostRequest(galleryId, postNo, this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public CommentListRequest CreateCommentListRequest(string galleryId, int postNo)
        {
            return new CommentListRequest(galleryId, postNo, this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public UpvoteRequest CreateUpvoteRequest(string galleryId, int postNo)
        {
            return new UpvoteRequest(galleryId, postNo, this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public DownvoteRequest CreateDownvoteRequest(string galleryId, int postNo)
        {
            return new DownvoteRequest(galleryId, postNo, this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public PostDeleteRequest CreatePostDeleteRequest(string galleryId, int postNo)
        {
            return new PostDeleteRequest(galleryId, postNo, this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public GallerySearchRequest CreateGallerySearchRequest(string keyword)
        {
            return new GallerySearchRequest(keyword, this);
        }

        public PostSearchRequest CreatePostSearchRequest(string galleryId, string keyword, SearchType searchType = SearchType.All)
        {
            return new PostSearchRequest(galleryId, keyword, searchType, this);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
