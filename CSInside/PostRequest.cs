using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
#nullable enable
    public class PostRequest : RequestBase<Post?>
#nullable restore
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
        internal PostRequest(string galleryId, int postNo, ApiService service) : base(service)
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
        public override async Task<Post?> ExecuteAsync()
#nullable restore
        {
            //HTTP 요청
            string appid = AuthTokenProvider.GetAppId();
            string hash = $"http://app.dcinside.com/api/gall_view_new.php?id={galleryId}&no={postNo}&app_id={appid}".ToBase64String(Encoding.ASCII);
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
            {
                throw new CSInsideException($"예기치 않은 오류: 서버에서 빈 문자열을 반환하였습니다.");
            }

            //예외처리
            JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (string)jObject["cause"] == "글없음")
                // JObject: {"result": false, "cause": "글없음"}
                return null;
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (jObject["cause"].ToString()?.Contains("성인 관련 갤러리") ?? false))
                // JObject: {"result": false, "cause": "앱스토어의 약관, 정책 준수 필요성에 따라 성인 인증이 필요한 갤러리들의 접속이 제한되게 됩니다. 이용에 불편을 드려 죄송합니다. 성인 관련 갤러리를 이용하실 분들께서는 모바일웹(m.dcinside.com)을 이용해주시기 바랍니다. 감사합니다."}
                throw new CSInsideException($"디시인사이드 API는 성인 갤러리 관련 기능을 제공하지 않습니다.");
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (jObject["cause"].ToString()?.Contains("운영원칙 위반") ?? false))
                // JObject: {"result": false, "cause": "해당 마이너 갤러리는 운영원칙 위반으로 접근이 제한되었습니다.\n매니저가 위반 게시물 정리 시 해당 갤러리는 다시 이용 가능합니다."}
                throw new CSInsideException($"운영원칙 위반으로 접근이 제한된 갤러리({galleryId})입니다.");
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause"))
                // JObject: {"result": false, "cause": ""}
                throw new CSInsideException($"알 수 없는 오류: {jObject["cause"]} / (Json: {jObject.ToString(Formatting.None)}, Raw: {jsonString.ToBase64String(Encoding.UTF8)})");
            if (!jObject.ContainsKey("view_info"))
                // JObject:
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 view_info 키를 찾을 수 없습니다. / (Json: {jObject.ToString(Formatting.None)}, Raw: {jsonString.ToBase64String(Encoding.UTF8)})");

            //반환값 처리
            Post post = jObject["view_info"].ToObject<Post>();
            post.GalleryId = galleryId;
            post.Body = HttpUtility.HtmlDecode(jObject["view_main"]["memo"].ToString());
            post.UpvoteCount = int.Parse(jObject["view_main"]["recommend"].ToString());
            post.MemberUpvoteCount = int.Parse(jObject["view_main"]["recommend_member"].ToString());
            post.DownvoteCount = int.Parse(jObject["view_main"]["nonrecommend"].ToString());
            return post;
        }
    }
}
