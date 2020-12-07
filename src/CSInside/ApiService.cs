using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    public class ApiService : ServiceBase, IGalleryService, IPostService, ICommentService
    {
        public ApiService(IAuthTokenProvider authTokenProvider) : base(authTokenProvider) { }

        public ApiService(IAuthTokenProvider authTokenProvider, IWebProxy webProxy) : base(authTokenProvider, webProxy) { }

        #region IGalleryService
        public IReader<PostHeader[]> CreatePostListReader(string galleryId)
        {
            throw new NotImplementedException();
        }

        public IReader<PostHeader[]> CreatePostSearchResultReader(string galleryId, string keyword, SearchType searchType)
        {
            return new PostSearchResultReader(galleryId, keyword, searchType, this);
        }
        #endregion

        #region IPostService
        public IReader<Post> CreatePostReader(string galleryId, int postNo)
        {
            return new PostReader(galleryId, postNo, this);
        }

        public IRequest CreatePostWriteRequest(string galleryId, string title, string nickname, string password, PostContent content)
        {
            return new PostWriteRequest(galleryId, nickname, password, title, content, this);
        }

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo)
        {
            return new PostDeleteRequest(galleryId, postNo, this);
        }

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo, string password)
        {
            return new PostDeleteRequest(galleryId, postNo, password, this);
        }

        public IRequest CreatePostUpvoteRequest(string galleryId, int postNo)
        {
            return new PostUpvoteRequest(galleryId, postNo, this);
        }

        public IRequest CreatePostDownvoteRequest(string galleryId, int postNo)
        {
            return new PostDownvoteRequest(galleryId, postNo, this);
        }
        #endregion

        #region ICommentService
        public IReader<Comment[]> CreateCommentReader(string galleryId, int postNo)
        {
            return new CommentReader(galleryId, postNo, this);
        }

        public IRequest CreateCommentWriteRequest(string galleryId, int postNo)
        {
            throw new NotImplementedException();
        }

        public IRequest CreateCommentDeleteRequest(string galleryId, int postNo, int commentNo, string password)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
