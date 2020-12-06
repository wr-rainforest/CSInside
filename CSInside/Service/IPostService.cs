﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    interface IPostService
    {
        public IReader<Post> CreatePostReader(string galleryId, int postNo);

        public IRequest CreatePostWriteRequest(string galleryId, string nickname, string password, object arg);

        public IRequest CreatePostDeleteRequest(string galleryId, int postNo);

        public IRequest CreatePostUpvoteRequest(string galleryId, int postNo);

        public IRequest CreatePostDownvoteRequest(string galleryId, int postNo);
    }
}