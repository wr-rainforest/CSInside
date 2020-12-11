using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 게시글 삭제 API 요청을 나타냅니다.
    /// </summary>
    public class PostDeleteRequest : RequestBase
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public RequestContent Content { get; }

        #region ctor
        internal PostDeleteRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal PostDeleteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Content = new RequestContent()
            {
                GalleryId = galleryId,
                PostNo = postNo
            };
        }

        internal PostDeleteRequest(string galleryId, int postNo, string password, ApiService service) : base(service)
        {
            Content = new RequestContent()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                Password = password
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
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (Content.PostNo == default)
                throw new CSInsideException("'Content.PostNo'의 값을 설정해주세요. ");
            if (Content.PostNo < 1)
                throw new CSInsideException("'Content.PostNo'의 값은 1 이상이어야 합니다.");

            string galleryId = Content.GalleryId;
            int postNo = Content.PostNo;
            string password = Content.Password;
            if (password == null)
                await DeleteMemberPost(galleryId, postNo);
            else
                await DeleteAnonymousPost(galleryId, postNo, password);
        }

        private async Task DeleteAnonymousPost(string galleryId, int postNo, string password)
        {
            string appId = base.AuthTokenProvider.GetAccessToken();
            string client_token = base.AuthTokenProvider.GetClientToken();
            string write_pw = password;
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("mode", "board_del");
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("no", postNo.ToString());
            keyValuePairs.Add("app_id", appId);
            keyValuePairs.Add("write_pw", write_pw);
            keyValuePairs.Add("client_token", client_token);
            await ExecuteAsync(keyValuePairs);
        }

        private async Task DeleteMemberPost(string galleryId, int postNo)
        {
            string appId = base.AuthTokenProvider.GetAccessToken();
            string client_token = base.AuthTokenProvider.GetClientToken();
            string user_id = base.AuthTokenProvider.GetUserToken();
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("mode", "board_del");
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("no", postNo.ToString());
            keyValuePairs.Add("app_id", appId);
            keyValuePairs.Add("user_id", user_id);
            keyValuePairs.Add("client_token", client_token);
            await ExecuteAsync(keyValuePairs);
        }

        private async Task ExecuteAsync(Dictionary<string, string> keyValuePairs)
        {
            // HTTP 요청 생성
            string uri = "http://app.dcinside.com/api/gall_del.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new FormUrlEncodedContent(keyValuePairs);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 예외처리            
            if (!jObject.ContainsKey("result"))
            // 
            throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 result 키를 찾을 수 없습니다.");

            // 반환값 처리
            if ((bool)jObject["result"])
                // {"result": true}
                return;
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("비밀번호 오류"))
                // {"result": false, "cause": "비밀번호 오류"}
                throw new CSInsideException((string)jObject["cause"]);
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("이미 삭제"))
                // {"result": false, "cause": "이미 삭제되었습니다."}
                throw new CSInsideException((string)jObject["cause"]);
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("권한 오류"))
                // {"result": false, "cause": "권한 오류"}
                throw new CSInsideException($"삭제 권한이 존재하지 않습니다.");

            throw new CSInsideException($"예기치 않은 오류: 응답 처리에 실패하였습니다.{jObject.ToString(Formatting.None)}");
        }

        public class RequestContent
        {
            /// <summary>
            /// 갤러리 ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 삭제할 게시글의 번호입니다. (필수 요청 변수)
            /// </summary>
            public int PostNo { get; set; }
#nullable enable
            /// <summary>
            /// 유동 게시글의 비밀번호입니다. (선택 요청 변수)
            /// </summary>
            public string? Password { get; set; }
#nullable restore
            internal RequestContent() { }
        }
    }
}