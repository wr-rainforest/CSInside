using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
    /// <summary>
    /// 갤러리 내 게시글 검색 API 요청을 나타냅니다.
    /// </summary>
#nullable enable
    public class PostSearchRequest : RequestBase<PostSearchResult?>
#nullable restore
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public RequestContent Content { get; }

        #region ctor
        internal PostSearchRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal PostSearchRequest(string galleryId, string keyword, SearchType searchType, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, keyword, searchType);
        }

        internal PostSearchRequest(string galleryId, string keyword, SearchType searchType, int from, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, keyword, searchType, from);
        }

        internal PostSearchRequest(string galleryId, string keyword, SearchType searchType, int from, int pageNo, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, keyword, searchType, from, pageNo);
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
#nullable enable
        public override async Task<PostSearchResult?> ExecuteAsync()
#nullable restore
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId' 는 필수 요청 변수입니다.");
            if (string.IsNullOrEmpty(Content.Keyword))
                throw new CSInsideException("'Content.Keyword' 값이 설정되지 않았습니다");
            if (Content.PageNo < 1)
                throw new CSInsideException("'Content.PageNo'는 1 이상이어야 합니다.");

            // 변수 초기화
            string galleryId = Content.GalleryId;
            string keyword = Content.Keyword;
            SearchType searchType = Content.SearchType;
            string s_type = searchType switch
            {
                SearchType.All => "all",
                SearchType.Title => "subject",
                SearchType.Content => "memo",
                SearchType.Writer => "name",
                SearchType.TitleContent => "subject_m",
                _ => throw new NotImplementedException("enum")
            };
            int? _ser_pos = (Content.From == null) ? null : -Content.From - 10000;
            int _pageNo = Content.PageNo;

            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string hash = Uri.EscapeDataString($"http://app.dcinside.com/api/gall_list_new.php?id={galleryId}&page={_pageNo}&app_id={app_id}&s_type={s_type}&serVal={keyword}{(_ser_pos == null ? string.Empty : $"&ser_pos={_ser_pos}")}").ToBase64String(Encoding.ASCII);
            string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 반환값 처리
            if (jObject.ContainsKey("result") && !(bool)jObject["result"] && ((string)jObject["cause"]).Contains("no gall"))
                return null;
            var postHeaders = jObject["gall_list"].ToObject<List<PostHeader>>();
            int ser_pos = (int)jObject["gall_info"][0]["ser_pos"];
            int pageCount = (int)jObject["gall_info"][0]["ser_total_page"];
            postHeaders.ForEach(item => { item.GalleryId = galleryId; });

            int from;
            int to;
            if (_ser_pos == null)
            {
                from = -ser_pos;
                to = -ser_pos + 10000;
            }
            else
            {
                from = -(int)_ser_pos - 10000;
                to = -(int)_ser_pos;
            }

            var result = new PostSearchResult((from, to), _pageNo, pageCount, postHeaders.ToArray());
            return result;
        }

        public class RequestContent
        {
            /// <summary>
            /// 갤러리ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 검색어 (필수 요청 변수)
            /// </summary>
            public string Keyword { get; set; }

            /// <summary>
            /// 검색 유형 (필수 요청 변수)
            /// </summary>
            public SearchType SearchType { get; set; }
#nullable enable
            /// <summary>
            /// 검색을 시작할 위치입니다. (선택 요청 변수)
            /// </summary>
            public int? From { get; set; }
#nullable restore
            /// <summary>
            /// 검색 범위 내 위치를 지정합니다. (필수 요청 변수) 기본값은 1입니다.
            /// </summary>
            public int PageNo { get; set; }

            #region ctor
            internal RequestContent() 
            {
                SearchType = SearchType.All;
                From = null;
                PageNo = 1;
            }

            internal RequestContent(string galleryId, string keyword, SearchType searchType)
            {
                GalleryId = galleryId;
                Keyword = keyword;
                SearchType = searchType;
                From = null;
                PageNo = 1;
            }

            internal RequestContent(string galleryId, string keyword, SearchType searchType, int from)
            {
                GalleryId = galleryId;
                Keyword = keyword;
                SearchType = searchType;
                From = from;
                PageNo = 1;
            }

            internal RequestContent(string galleryId, string keyword, SearchType searchType, int from, int pageNo)
            {
                GalleryId = galleryId;
                Keyword = keyword;
                SearchType = searchType;
                From = from;
                PageNo = pageNo;
            }
            #endregion
        }
    }
}
