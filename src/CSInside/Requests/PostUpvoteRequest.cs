using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 게시글 추천 API 요청을 나타냅니다. 
    /// </summary>
    public class PostUpvoteRequest : RequestBase
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public Content Params { get; }

        #region ctor
        internal PostUpvoteRequest(ApiService service) : base(service)
        {
            Params = new Content();
        }

        internal PostUpvoteRequest(string galleryId, int postNo, ApiService service) : base(service)
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
            string appId = base.AuthTokenProvider.GetAccessToken();
            string uri = "http://app.dcinside.com/api/_recommend_up.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("no", postNo.ToString());
            keyValuePairs.Add("app_id", appId);
            request.Content = new FormUrlEncodedContent(keyValuePairs);

            // 전송
            var task =  base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 예외처리
            if (!jObject.ContainsKey("result"))
                // 
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 result 키를 찾을 수 없습니다.");
            if (!jObject.ContainsKey("cause"))
                //
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 cause 키를 찾을 수 없습니다.");
            
            // 반환값 처리
            if ((bool)jObject["result"])
                // {"result": true, "cause": "추천 하였습니다.", "member": ""}
                return;
            else if (!(bool)jObject["result"])
                // {"result": false, "cause": "추천은 1일 1회만 가능합니다."}
                // {"result": false, "cause": "추천 할수없습니다."}
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
