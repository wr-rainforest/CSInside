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
    public class PostDeleteRequest : RequestBase<bool?>
    {
        private readonly string galleryId;

        private readonly int postNo;

        public string GalleryId { get => galleryId; }

        public int PostNo { get => postNo; }

        /// <exception cref="ArgumentNullException"></exception>
        internal PostDeleteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            // 매개변수 검사
            if (galleryId is null)
                throw new ArgumentNullException(nameof(galleryId));

            // 필드 초기화
            this.galleryId = galleryId;
            this.postNo = postNo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task<bool?> ExecuteAsync()
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
            return await ExecuteAsync(keyValuePairs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public async Task<bool?> ExecuteAsync(string password)
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
            return await ExecuteAsync(keyValuePairs);
        }

        private async Task<bool?> ExecuteAsync(Dictionary<string, string> keyValuePairs)
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
                return true;
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("비밀번호 오류"))
                // {"result": false, "cause": "비밀번호 오류"}
                return false;
            if (!(bool)jObject["result"] && ((string)jObject["cause"]).Contains("이미 삭제"))
                // {"result": false, "cause": "이미 삭제되었습니다."}
                return null;
            throw new CSInsideException($"예기치 않은 오류: 응답 처리에 실패하였습니다.{jObject.ToString(Formatting.None)}");
        }
    }
}