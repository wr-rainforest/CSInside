using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
    /// <summary>
    /// 게시글 목록 요청 API를 나타냅니다.
    /// </summary>
    public class PostListRequest : RequestBase<PostHeader[]>
    {
        /// <summary>
        /// 요청 변수를 가져오거나 설정합니다.
        /// </summary>
        public Content Params { get; set; }

        #region ctor
        internal PostListRequest(ApiService service) : base(service)
        {
            Params = new Content()
            {
                PageNo = 1
            };
        }

        internal PostListRequest(string galleryId, int pageNo, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PageNo = pageNo
            };
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns>게시글 목록입니다.</returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task<PostHeader[]> ExecuteAsync()
        {
            // Content 값 검증
            if (string.IsNullOrEmpty(Params.GalleryId))
                throw new CSInsideException("");
            if (Params.PageNo < 0)
                throw new CSInsideException("");

            // 변수 초기화
            string app_id = AuthTokenProvider.GetAccessToken();
            string galleryId = Params.GalleryId;
            int pageNo = Params.PageNo;

            // HTTP 요청 생성
            string hash = $"http://app.dcinside.com/api/gall_list_new.php?id={galleryId}&page={pageNo}&app_id={app_id}".ToBase64String(Encoding.UTF8);
            string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}" ;
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 예외처리
            if (jObject.ContainsKey("result") && !(bool)jObject["result"] && ((string)jObject["cause"]).Contains("no gall"))
                return null;
            

            // 반환값 처리
            return jObject["gall_list"].ToObject<PostHeader[]>();
        }

        public class Content
        {
            /// <summary>
            /// 갤러리 ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 페이지 번호 (필수 요청 변수)
            /// </summary>
            public int PageNo { get; set; }

            internal Content() { }
        }
    }
}
