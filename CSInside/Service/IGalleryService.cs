using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    interface IGalleryService
    {
#nullable enable
        public IReader<PostHeader[]?> CreatePostListReader(string galleryId);

        public IReader<PostHeader[]?> CreatePostSearchResultReader(string galleryId, string keyword, SearchType searchType);
#nullable restore
    }
}
