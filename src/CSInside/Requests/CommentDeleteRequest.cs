using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CSInside
{
    /// <summary>
    /// 댓글 삭제 API 요청을 나타냅니다.
    /// </summary>
    public class CommentDeleteRequest : RequestBase
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public RequestContent Content { get; }

        #region ctor
        internal CommentDeleteRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal CommentDeleteRequest(string galleryId, int postNo, int commentNo, ApiService service) : base(service)
        {
            Content = new RequestContent()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                CommentNo = commentNo
            };
        }

        internal CommentDeleteRequest(string galleryId, int postNo, int commentNo, string password, ApiService service) : base(service)
        {
            Content = new RequestContent()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                CommentNo = commentNo,
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
            if (Content.CommentNo == default)
                throw new CSInsideException("'Content.CommentNo'의 값을 설정해주세요. ");
            if (Content.CommentNo < 1)
                throw new CSInsideException("'Content.CommentNo'의 값은 1 이상이어야 합니다.");

            // 초기화
            bool isGuest = Content.Password != null;
            string comment_pw = Content.Password;
            string client_token = AuthTokenProvider.GetClientToken();
            string id = Content.GalleryId;
            int no = Content.PostNo;
            string board_id = "";
            string mode = "comment_del";
            string best_chk = "N";
            string best_comno = "0";
            int comment_no = Content.CommentNo;
            string app_id = AuthTokenProvider.GetAccessToken();

            // HTTP 요청 생성
            string uri = "http://app.dcinside.com/api/comment_del.php ";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            if (isGuest)
            {
                keyValuePairs.Add("comment_pw", comment_pw);
            }
            else
            {
                keyValuePairs.Add("user_id", AuthTokenProvider.GetUserToken());
            }
            keyValuePairs.Add("client_token", client_token);
            keyValuePairs.Add("id", id);
            keyValuePairs.Add("no", no.ToString());
            keyValuePairs.Add("board_id", board_id);
            keyValuePairs.Add("mode", mode);
            keyValuePairs.Add("best_chk", best_chk);
            keyValuePairs.Add("best_comno", best_comno);
            keyValuePairs.Add("comment_no", comment_no.ToString());
            keyValuePairs.Add("app_id", app_id);
            var contents = new MultipartFormDataContent();
            keyValuePairs.ToList().ForEach(item => contents.Add(new StringContent(item.Value), item.Key));
            request.Content = contents;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            
            if (!jObject.ContainsKey("result"))
                throw new CSInsideException(jObject.ToString(Formatting.None));

            if ((bool)jObject["result"])
                return;
            else
                throw new CSInsideException((string)jObject["cause"]);
        }

        public class RequestContent
        {
            /// <summary>
            /// 갤러리 ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 게시글 번호 (필수 요청 변수)
            /// </summary>
            public int PostNo { get; set; }

            /// <summary>
            /// 댓글 번호 (필수 요청 변수)
            /// </summary>
            public int CommentNo { get; set; }
#nullable enable
            /// <summary>
            /// 유동 댓글의 비밀번호입니다. (선택 요청 변수)
            /// </summary>
            public string? Password { get; set; }
#nullable restore
            internal RequestContent() { }
        }
    }
}
