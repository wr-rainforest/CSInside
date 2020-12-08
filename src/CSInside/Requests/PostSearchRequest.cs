using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
#nullable enable
    public class PostSearchRequest : RequestBase<PostSearchResult?>
#nullable restore
    {
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

        #region public override async Task<PostSearchResult?> ExecuteAsync()
#nullable enable
        public override async Task<PostSearchResult?> ExecuteAsync()
#nullable restore
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (string.IsNullOrEmpty(Content.Keyword))
                throw new CSInsideException("'Content.Keyword'의 값을 설정해주세요. ");
            if (Content.PageNo < 1)
                throw new CSInsideException("'Content.PageNo'의 값은 1 이상이어야 합니다.");

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
        #endregion

        #region public class RequestContent
        public class RequestContent
        {
            public string GalleryId { get; set; }

            public string Keyword { get; set; }

            public SearchType SearchType { get; set; }
#nullable enable
            public int? From { get; set; }
#nullable restore
            public int PageNo { get; set; }

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
        }
        #endregion
    }
}
