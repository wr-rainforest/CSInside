using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    internal class PostDeleteRequest : RequestBase
    {
        private readonly string galleryId;

        private readonly int postNo;

        #region Anonymous
#nullable enable
        private readonly string? password;
#nullable restore
        internal PostDeleteRequest(string galleryId, int postNo, string password, ApiService service) : this(galleryId, postNo, service)
        {
            this.password = password;
        }

        private async Task DeleteAnonymousPost()
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
        #endregion

        #region Member
        internal PostDeleteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            this.galleryId = galleryId;
            this.postNo = postNo;
        }

        private async Task DeleteMemberPost()
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
        #endregion

        /// <exception cref="CSInsideException"></exception>
        public override async Task ExecuteAsync()
        {
            if (password == null)
                await DeleteMemberPost();
            else
                await DeleteAnonymousPost();
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
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("권한 오류"))
                // {"result": false, "cause": "권한 오류"}
                throw new CSInsideException($"삭제 권한이 존재하지 않습니다.");

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

            throw new CSInsideException($"예기치 않은 오류: 응답 처리에 실패하였습니다.{jObject.ToString(Formatting.None)}");
        }
    }
}