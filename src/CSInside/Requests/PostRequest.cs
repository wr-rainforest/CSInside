using System;
using System.Net.Http;
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
        public RequestContent Content { get; }

        #region internal ctor
        internal PostRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal PostRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, postNo);
        }
        #endregion

        #region public override async Task<Post?> ExecuteAsync()
#nullable enable
        public override async Task<Post?> ExecuteAsync()
#nullable restore
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (Content.PostNo == default)
                throw new CSInsideException("'Content.PostNo'의 값을 설정해주세요. ");
            if (Content.PostNo < 1)
                throw new CSInsideException("'Content.PostNo'의 값은 1 이상이어야 합니다.");

            // 변수 초기화
            string galleryId = Content.GalleryId;
            int postNo = Content.PostNo;

            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string hash = Uri.EscapeUriString($"http://app.dcinside.com/api/gall_view_new.php?id={galleryId}&no={postNo}&app_id={app_id}").ToBase64String(Encoding.ASCII);
            string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 예외 처리
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (string)jObject["cause"] == "글없음")
                // JObject: {"result": false, "cause": "글없음"}
                return null;
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (jObject["cause"].ToString()?.Contains("성인 관련 갤러리") ?? false))
                // JObject: {"result": false, "cause": "앱스토어의 약관, 정책 준수 필요성에 따라 성인 인증이 필요한 갤러리들의 접속이 제한되게 됩니다. 이용에 불편을 드려 죄송합니다. 성인 관련 갤러리를 이용하실 분들께서는 모바일웹(m.dcinside.com)을 이용해주시기 바랍니다. 감사합니다."}
                return null;
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause") && (jObject["cause"].ToString()?.Contains("운영원칙 위반") ?? false))
                // JObject: {"result": false, "cause": "해당 마이너 갤러리는 운영원칙 위반으로 접근이 제한되었습니다.\n매니저가 위반 게시물 정리 시 해당 갤러리는 다시 이용 가능합니다."}
                return null;
            if (jObject.ContainsKey("result") && jObject.ContainsKey("cause"))
                // JObject: {"result": false, "cause": ""}
                throw new CSInsideException($"예기치 않은 오류: 응답 분석에 실패하였습니다. {jObject.ToString(Formatting.None)}");
            if (!jObject.ContainsKey("view_info"))
                // JObject:
                throw new CSInsideException($"예기치 않은 오류: 응답 본문에서 view_info 키를 찾을 수 없습니다. {jObject.ToString(Formatting.None)}");

            // 반환값 처리
            Post post = jObject["view_info"].ToObject<Post>();
            post.GalleryId = galleryId;
            post.Body = HttpUtility.HtmlDecode(jObject["view_main"]["memo"].ToString());
            post.UpvoteCount = int.Parse(jObject["view_main"]["recommend"].ToString());
            post.MemberUpvoteCount = int.Parse(jObject["view_main"]["recommend_member"].ToString());
            post.DownvoteCount = int.Parse(jObject["view_main"]["nonrecommend"].ToString());
            return post;
        }
        #endregion

        #region public class RequestContent
        public class RequestContent
        {
            public string GalleryId { get; set; }

            public int PostNo { get; set; }

            internal RequestContent() { }

            internal RequestContent(string galleryId, int postNo)
            {
                GalleryId = galleryId;
                PostNo = postNo;
            }
        }
        #endregion
    }
}
