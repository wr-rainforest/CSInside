using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 게시글 검색 결과를 나타냅니다.
    /// </summary>
    public class PostSearchResult
    {
        /// <summary>
        /// 검색 범위
        /// </summary>
        public (int From, int To) Range { get; }

        /// <summary>
        /// 검색 범위 내 현재 페이지 번호
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// 검색 범위 내 페이지 개수
        /// </summary>
        public int PageCount { get; }

        /// <summary>
        /// 게시글 헤더 목록
        /// </summary>
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
