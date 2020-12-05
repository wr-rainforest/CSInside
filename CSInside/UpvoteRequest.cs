using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class UpvoteRequest : RequestBase<bool>
    {
        private readonly string galleryId;

        private readonly int postNo;

        public string GalleryId { get => galleryId; }

        public int PostNo { get => postNo; }

        /// <exception cref="ArgumentNullException"></exception>
        internal UpvoteRequest(string galleryId, int postNo , ApiService service) : base(service)
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
        public override async Task<bool> ExecuteAsync()
        {
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
                return true;
            else if (!(bool)jObject["result"])
                // {"result": false, "cause": "추천은 1일 1회만 가능합니다."}
                // {"result": false, "cause": "추천 할수없습니다."}
                return false;
            else
                throw new Exception();
        }
    }
}
