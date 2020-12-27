using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 게시글 비추천 API 요청을 나타냅니다.
    /// </summary>
    public class PostDownvoteRequest : RequestBase
    {
        /// <summary>
        /// 요청 변수를 가져오거나 설정합니다.
        /// </summary>
        public Content Params { get; set; }

        #region ctor
        internal PostDownvoteRequest(ApiService service) : base(service)
        {
            Params = new Content();
        }

        internal PostDownvoteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PostNo = postNo
            };
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task ExecuteAsync()
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Params.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (Params.PostNo == default)
                throw new CSInsideException("'Content.PostNo'의 값을 설정해주세요. ");
            if (Params.PostNo < 1)
                throw new CSInsideException("'Content.PostNo'의 값은 1 이상이어야 합니다.");

            // 변수 초기화
            string galleryId = Params.GalleryId;
            int postNo = Params.PostNo;

            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string uri = "http://app.dcinside.com/api/_recommend_down.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("no", postNo.ToString());
            keyValuePairs.Add("app_id", app_id);
            request.Content = new FormUrlEncodedContent(keyValuePairs);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 반환값 처리
            if ((bool)jObject["result"])
                // {"result": true, "cause": "추천 하였습니다.", "member": ""}
                return;
            else if (!(bool)jObject["result"])
                // {"result": false, "cause": "비추천은 1일 1회만 가능합니다."}
                // {"result": false, "cause": "비추천 할수 없습니다."}
                throw new CSInsideException((string)jObject["cause"]);
            else
                throw new Exception();
        }

        public class Content
        {
            /// <summary>
            /// 갤러리 ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 게시글 번호 (필수 요청 변수)
            /// </summary>
            public int PostNo { get; set; }

            internal Content() { }
        }
    }
}
