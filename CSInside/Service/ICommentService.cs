using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    interface ICommentService
    {
        public IReader<Comment[]> CreateCommentReader(string galleryId, int PostNo);

        public IRequest CreateCommentWriteRequest(string galleryId, int postNo);

        public IRequest CreateCommentDeleteRequest(string galleryId, int postNo, int commentNo, string password);
    }
}
