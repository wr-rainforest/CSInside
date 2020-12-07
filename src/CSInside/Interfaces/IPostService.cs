using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    interface IPostService
    {
#nullable enable
        public IReader<Post?> CreatePostReader(string galleryId, int postNo);
#nullable restore
        public IRequest CreatePostWriteRequest(string galleryId, string nickname, string password, string title, PostContent content);

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo);

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo, string password);

        public IRequest CreatePostUpvoteRequest(string galleryId, int postNo);

        public IRequest CreatePostDownvoteRequest(string galleryId, int postNo);
    }
}
