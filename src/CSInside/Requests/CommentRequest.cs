using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;

namespace CSInside
{
    /// <summary>
    /// 댓글 조회 API 요청을 나타냅니다.
    /// </summary>
#nullable enable
    public class CommentRequest : RequestBase<Comment[]?>
#nullable restore
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public RequestContent Content { get; }

        #region ctor
        internal CommentRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal CommentRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Content = new RequestContent()
            {
                GalleryId = galleryId,
                PostNo = postNo
            };
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns>댓글 목록입니다.
        /// <br></br>
        /// <br></br>
        /// 게시글이 존재하지 않거나, 댓글이 0개일 경우 null을 반환합니다.</returns>
        /// <exception cref="CSInsideException"></exception>
#nullable enable
        public override async Task<Comment[]?> ExecuteAsync()
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

            List<Comment> comments = new List<Comment>();
            for (int i = 1, pageCount = 1; i <= pageCount; i++)
            {
                // HTTP 요청 생성
                string app_id = base.AuthTokenProvider.GetAccessToken();
                string hash = Uri.EscapeUriString($"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={postNo}&re_page={i}&app_id={app_id}").ToBase64String(Encoding.ASCII);
                string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}";
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                // 전송
                var task = base.GetResponseAsync(request);

                // 응답 수신
                JObject jObject = await task;

                // 예외 처리
                if (jObject.ContainsKey("result") && jObject.ContainsKey("cause"))
                    // JObject: {"result": false, "cause": ""}
                    throw new CSInsideException($"알 수 없는 오류: {jObject.ToString(Formatting.None)}");
                if (jObject.ContainsKey("message"))
                    //JObject: {"message": false, "code": 500}
                    throw new CSInsideException($"알 수 없는 오류: {jObject.ToString(Formatting.None)}");

                // 상태 정의
                pageCount = (int)jObject["total_page"];

                // 반환값 처리
                comments.AddRange(jObject["comment_list"].ToObject<List<Comment>>());
            }
            if (comments.Count == 0)
                return null;
            comments.ForEach(item => { item.GalleryId = galleryId; item.PostNo = postNo; });
            return comments.Distinct().ToArray();
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

            internal RequestContent() { }
        }
    }
}
