using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 댓글 작성 API 요청을 나타냅니다.
    /// </summary>
    public class CommentWriteRequest : RequestBase<int>
    {
        /// <summary>
        /// 요청 변수를 가져오거나 설정합니다.
        /// </summary>
        public Content Params { get; set; }

        #region ctor
        internal CommentWriteRequest(ApiService service) : base(service)
        {
            Params = new Content();
        }

        internal CommentWriteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PostNo = postNo
            };
        }

        internal CommentWriteRequest(string galleryId, int postNo, Paragraph paragraph, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                Paragraph = paragraph
            };
        }

        internal CommentWriteRequest(string galleryId, int postNo, string nickname, string password, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                Nickname = nickname,
                Password = password
            };
        }

        internal CommentWriteRequest(string galleryId, int postNo, string nickname, string password, Paragraph paragraph, ApiService service) : base(service)
        {
            Params = new Content()
            {
                GalleryId = galleryId,
                PostNo = postNo,
                Nickname = nickname,
                Password = password,
                Paragraph = paragraph
            };
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns>작성한 댓글의 번호입니다.</returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task<int> ExecuteAsync()
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Params.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (Params.PostNo == default)
                throw new CSInsideException("'Content.PostNo'의 값을 설정해주세요. ");
            if (Params.PostNo < 1)
                throw new CSInsideException("'Content.PostNo'의 값은 1 이상이어야 합니다.");
            if (Params.Paragraph == null)
                throw new CSInsideException("'Content.Paragraph'의 값을 설정해 주세요.");
            bool isAnonymous = !string.IsNullOrEmpty(Params.Nickname) && !string.IsNullOrEmpty(Params.Password);

            // 변수 초기화
            string galleryId = Params.GalleryId;
            int postNo = Params.PostNo;
            string nickname = null;
            string password = null;
            if (isAnonymous)
            {
                nickname = Params.Nickname;
                password = Params.Password;
            }
            Paragraph paragraph = Params.Paragraph;

            // HTTP 요청 생성
            string appId = base.AuthTokenProvider.GetAccessToken();
            string client_token = base.AuthTokenProvider.GetClientToken();
            string uri = "http://app.dcinside.com/api/comment_ok.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("no", postNo.ToString());
            keyValuePairs.Add("board_id", "");
            keyValuePairs.Add("best_cnk", "N");
            keyValuePairs.Add("best_comno", "0");
            keyValuePairs.Add("mode", "com_write");
            if (isAnonymous)
            {
                keyValuePairs.Add("comment_nick", HttpUtility.HtmlEncode(nickname));
                keyValuePairs.Add("comment_pw", password);
            }
            else
            {
                string user_id = AuthTokenProvider.GetUserToken();
                keyValuePairs.Add("user_id", user_id);
            }
            keyValuePairs.Add("app_id", appId);
            keyValuePairs.Add("client_token", client_token);
            var contents = new MultipartFormDataContent();
            keyValuePairs.ToList().ForEach(item => contents.Add(new StringContent(item.Value), item.Key));
            switch (paragraph)
            {
                case StringParagraph strpar:
                    contents.Add(new StringContent(strpar.Text), "comment_memo");
                    break;
                case DCConParagraph dcconpar:
                    {
                        DCCon dccon = dcconpar.DCCon;
                        var idxContent = new StringContent(dccon.DetailIndex.ToString());
                        contents.Add(paragraph.GetHttpContent(), "comment_memo");
                        contents.Add(idxContent, "detail_idx");
                        break;
                    }
                default:
                    throw new CSInsideException("StringParagraph, DCConParagraph 이외의 파생 형식은 지원하지 않습니다.");
            }
            request.Content = contents;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            if ((bool)jObject["result"])
                return (int)jObject["data"];
            else
                throw new CSInsideException((string)jObject["cause"]);
        }

        public class Content
        {
            /// <summary>
            /// 갤러리 ID (필수 요청 변수)
            /// </summary>
            public string GalleryId { get; set; }

            /// <summary>
            /// 댓글을 작성할 게시글의 번호입니다. (필수 요청 변수)
            /// </summary>
            public int PostNo { get; set; }
#nullable enable
            /// <summary>
            /// 유동 댓글 작성시에 사용할 닉네임입니다. (선택 요청 변수)
            /// </summary>
            public string? Nickname { get; set; }

            /// <summary>
            /// 유동 댓글 작성에 사용할 비밀번호입니다. (선택 요청 변수)
            /// </summary>
            public string? Password { get; set; }
#nullable restore
            /// <summary>
            /// 댓글 본문입니다. (필수 요청 변수)<br></br><br></br>  <seealso cref="StringParagraph"/>, <seealso cref="DCConParagraph"/> 이외의 파생 형식은 지원하지 않습니다.
            /// </summary>
            public Paragraph Paragraph { get; set; }

            internal Content() { }
        }
    }
}
