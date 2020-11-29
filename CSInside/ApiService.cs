﻿using System;
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

        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ImageRequest CreateImageRequest(string imageUri)
        {
            return new ImageRequest(imageUri, this);
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

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
