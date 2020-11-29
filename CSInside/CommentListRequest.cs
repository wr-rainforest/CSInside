using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
    public class CommentListRequest : RequestBase<Comment[]>
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
        public CommentListRequest(string galleryId, int postNo, ApiService service) : base(service)
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
#nullable enable
        public override async Task<Comment[]?> ExecuteAsync()
#nullable restore
        {
            List<Comment> comments = new List<Comment>();
            for (int i = 1, totalPage = 1; i <= totalPage; i++)
            {
                //HTTP 요청
                string app_id = AuthTokenProvider.GetAccessToken();
                string hash = $"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={postNo}&re_page={i}&app_id={app_id}".ToBase64String(Encoding.ASCII);
                string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}");
                string jsonString;
                try
                {
                    var response = await Client.GetAsync(uri);
                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                        throw new CSInsideException($"API 서버에서 Internal Server Error를 반환하였습니다. 인증 토큰이 만료되었거나 올바르지 않은 인증 토큰일 수 있습니다.");
                    jsonString = await response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(CSInsideException))
                        throw;
                    throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
                }
                if (string.IsNullOrEmpty(jsonString))
                    throw new CSInsideException($"예기치 않은 오류: API 서버에서 빈 문자열을 반환하였습니다.");

                //예외처리
                JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
                if (jObject.ContainsKey("result") && jObject.ContainsKey("cause"))
                    // JObject: {"result": false, "cause": ""}
                    throw new CSInsideException($"알 수 없는 오류: {jObject["cause"]} / (Json: {jObject.ToString(Formatting.None)}, Raw: {jsonString.ToBase64String(Encoding.UTF8)})");
                if (jObject.ContainsKey("message"))
                    //JObject: {"message": false, "code": 500}
                    throw new CSInsideException($"알 수 없는 오류 (Json: {jObject} / Raw: {jsonString})");

                //반환값 처리
                totalPage = (int)jObject["total_page"];
                comments.AddRange(jObject["comment_list"].ToObject<Comment[]>());
            }
            comments.ForEach(item => { item.GalleryId = galleryId; item.PostNo = postNo; });
            if (comments.Count == 0)
                return null;
            return comments.Distinct().ToArray();
        }
    }
}
