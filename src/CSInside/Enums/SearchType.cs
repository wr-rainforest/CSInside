using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 검색 유형입니다.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// 전체
        /// </summary>
        All = 0,

        /// <summary>
        /// 제목
        /// </summary>
        Title = 1,

        /// <summary>
        /// 내용
        /// </summary>
        Content = 2,

        /// <summary>
        /// 글쓴이
        /// </summary>
        Writer = 3,

        /// <summary>
        /// 제목+내용
        /// </summary>
        TitleContent = 4
    }
}
