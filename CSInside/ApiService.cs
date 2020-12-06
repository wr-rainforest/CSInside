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

        public IReader<PostHeader[]> CreatePostSearchReader(string galleryId, string keyword, SearchType searchType)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IPostService
        public IReader<Post> CreatePostReader(string galleryId, int postNo)
        {
            throw new NotImplementedException();
        }

        public IRequest CreatePostWriteRequest(string galleryId, string nickname, string password, object arg)
        {
            throw new NotImplementedException();
        }

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo)
        {
            throw new NotImplementedException();
        }

        public IRequest CreatePostUpvoteRequest(string galleryId, int postNo)
        {
            throw new NotImplementedException();
        }

        public IRequest CreatePostDownvoteRequest(string galleryId, int postNo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ICommentService
        public IReader<Comment[]> CreateCommentReader(string galleryId, int PostNo)
        {
            throw new NotImplementedException();
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
