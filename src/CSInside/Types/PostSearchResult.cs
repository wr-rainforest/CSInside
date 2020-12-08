using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    public class PostSearchResult
    {
        public (int From, int To) Range { get; }

        public int CurrentPage { get; }

        public int PageCount { get; }

        public PostHeader[] PostHeaders { get; }

        public PostSearchResult((int From, int To) range, int currentPage, int pageCount, PostHeader[] postHeaders)
        {
            Range = range;
            CurrentPage = currentPage;
            PageCount = pageCount;
            PostHeaders = postHeaders;
        }
    }
}
