using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class DownvoteRequest : RequestBase<bool>
    {
        private readonly string galleryId;

        private readonly int postNo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="galleryId"></param>
        /// <param name="postNo"></param>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal DownvoteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            //매개변수 검사
            if (galleryId is null)
                throw new ArgumentNullException(nameof(galleryId));

            //필드 초기화
            this.galleryId = galleryId;
            this.postNo = postNo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task<bool> ExecuteAsync()
        {
            //HTTP 요청
            string appId = AuthTokenProvider.GetAppId();
            //string confirm_id = AuthTokenProvider.GetConfirmId();
            string uri = "http://app.dcinside.com/api/_recommend_down.php";
            string jsonString;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("id", galleryId);
                keyValuePairs.Add("no", postNo.ToString());
                keyValuePairs.Add("app_id", appId);
                //keyValuePairs.Add("confirm_id", confirm_id);
                request.Content = new FormUrlEncodedContent(keyValuePairs);
                var response = await Client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    throw new CSInsideException($"API 서버에서 Internal Server Error를 반환하였습니다. 인증 토큰이 만료되었거나 올바르지 않은 인증 토큰일 수 있습니다.");
                response.EnsureSuccessStatusCode();
                jsonString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CSInsideException))
                    throw;
                throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                throw new CSInsideException($"예기치 않은 오류: 서버에서 빈 문자열을 반환하였습니다.");
            }

            //예외처리
            JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
            if (!jObject.ContainsKey("result"))
                // 
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 result 키를 찾을 수 없습니다.");
            if (!jObject.ContainsKey("cause"))
                //
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 cause 키를 찾을 수 없습니다.");
            
            //반환값 처리
            if ((bool)jObject["result"])
                // {"result": true, "cause": "추천 하였습니다.", "member": ""}
                return true;
            else if (!(bool)jObject["result"])
                // {"result": false, "cause": "비추천은 1일 1회만 가능합니다."}
                // {"result": false, "cause": "비추천 할수 없습니다."}
                return false;
            else
                throw new Exception();
        }
    }
}
