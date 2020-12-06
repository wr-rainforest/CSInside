using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    interface IGalleryService
    {
        public IReader<PostHeader[]> CreatePostListReader(string galleryId);

        public IReader<PostHeader[]> CreatePostSearchReader(string galleryId, string keyword, SearchType searchType);
    }
}
