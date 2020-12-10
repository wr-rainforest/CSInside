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
        #region Property
        internal HttpClient Client { get; }

        internal IAuthTokenProvider AuthTokenProvider { get; }
        #endregion

        #region public ctor
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
        #endregion

        #region CreatePostListRequest
        public PostListRequest CreatePostListRequest()
        {
            return new PostListRequest(this);
        }

        public PostListRequest CreatePostListRequest(string galleryId, int pageNo)
        {
            return new PostListRequest(galleryId, pageNo, this);
        }
        #endregion

        #region CreatePostSearchRequest
        public PostSearchRequest CreatePostSearchRequest()
        {
            return new PostSearchRequest(this);
        }

        public PostSearchRequest CreatePostSearchRequest(string galleryId, string keyword, SearchType searchType)
        {
            return new PostSearchRequest(galleryId, keyword, searchType, this);
        }

        public PostSearchRequest CreatePostSearchRequest(string galleryId, string keyword, SearchType searchType, int from)
        {
            return new PostSearchRequest(galleryId, keyword, searchType, from, this);
        }

        public PostSearchRequest CreatePostSearchRequest(string galleryId, string keyword, SearchType searchType, int from, int pageNo)
        {
            return new PostSearchRequest(galleryId, keyword, searchType, from, pageNo, this);
        }
        #endregion

        #region CreatePostRequest
        public PostRequest CreatePostRequest()
        {
            return new PostRequest(this);
        }

        public PostRequest CreatePostRequest(string galleryId, int postNo)
        {
            return new PostRequest(galleryId, postNo, this);
        }
        #endregion

        #region CreatePostWriteRequest
        public PostWriteRequest CreatePostWriteRequest()
        {
            return new PostWriteRequest(this);
        }

        public PostWriteRequest CreatePostWriteRequest(string galleryId, string title)
        {
            return new PostWriteRequest(galleryId, title, this);
        }

        public PostWriteRequest CreatePostWriteRequest(string galleryId, string title, ParagraphCollection paragraphs)
        {
            return new PostWriteRequest(galleryId, title, paragraphs, this);
        }

        public PostWriteRequest CreatePostWriteRequest(string galleryId, string title, string nickname, string password)
        {
            return new PostWriteRequest(galleryId, title, nickname, password, this);
        }

        public PostWriteRequest CreatePostWriteRequest(string galleryId, string title, string nickname, string password, ParagraphCollection paragraphs)
        {
            return new PostWriteRequest(galleryId, title, nickname, password, paragraphs, this);
        }
        #endregion

        #region CreatePostDeleteRequest
        public PostDeleteRequest CreatePostDeleteRequest()
        {
            return new PostDeleteRequest(this);
        }

        public PostDeleteRequest CreatePostDeleteRequest(string galleryId, int postNo)
        {
            return new PostDeleteRequest(galleryId, postNo, this);
        }

        public PostDeleteRequest CreatePostDeleteRequest(string galleryId, int postNo, string password)
        {
            return new PostDeleteRequest(galleryId, postNo, password, this);
        }
        #endregion

        #region CreatePostUpvoteRequest
        public PostUpvoteRequest CreatePostUpvoteRequest()
        {
            return new PostUpvoteRequest(this);
        }

        public PostUpvoteRequest CreatePostUpvoteRequest(string galleryId, int postNo)
        {
            return new PostUpvoteRequest(galleryId, postNo, this);
        }
        #endregion

        #region CreatePostDownvoteRequest
        public PostDownvoteRequest CreatePostDownvoteRequest()
        {
            return new PostDownvoteRequest(this);
        }

        public PostDownvoteRequest CreatePostDownvoteRequest(string galleryId, int postNo)
        {
            return new PostDownvoteRequest(galleryId, postNo, this);
        }
        #endregion

        #region CreateCommentRequest
        public CommentRequest CreateCommentRequest()
        {
            return new CommentRequest(this);
        }

        public CommentRequest CreateCommentRequest(string galleryId, int postNo)
        {
            return new CommentRequest(galleryId, postNo, this);
        }
        #endregion

        #region CreateCommentWriteRequest
        public CommentWriteRequest CreateCommentWriteRequest()
        {
            return new CommentWriteRequest(this);
        }

        public CommentWriteRequest CreateCommentWriteRequest(string galleryId, int postNo)
        {
            return new CommentWriteRequest(galleryId, postNo, this);
        }

        public CommentWriteRequest CreateCommentWriteRequest(string galleryId, int postNo, Paragraph paragraph)
        {
            return new CommentWriteRequest(galleryId, postNo, paragraph, this);
        }

        public CommentWriteRequest CreateCommentWriteRequest(string galleryId, int postNo, string nickname, string password)
        {
            return new CommentWriteRequest(galleryId, postNo, nickname, password, this);
        }

        public CommentWriteRequest CreateCommentWriteRequest(string galleryId, int postNo, string nickname, string password, Paragraph paragraph)
        {
            return new CommentWriteRequest(galleryId, postNo, nickname, password, paragraph, this);
        }
        #endregion

        #region CreateCommentDeleteRequest
        public CommentDeleteRequest CreateCommentDeleteRequest()
        {
            return new CommentDeleteRequest(this);
        }

        public CommentDeleteRequest CreateCommentDeleteRequest(string galleryId, int postNo, int commentNo)
        {
            return new CommentDeleteRequest(galleryId, postNo, commentNo, this);
        }

        public CommentDeleteRequest CreateCommentDeleteRequest(string galleryId, int postNo, int commentNo, string password)
        {
            return new CommentDeleteRequest(galleryId, postNo, commentNo, password, this);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            ((IDisposable)Client).Dispose();
        }
        #endregion
    }
}
