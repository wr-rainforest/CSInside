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
    public class CommentWriteRequest : RequestBase<int>
    {
        public RequestContent Content { get; }

        #region internal ctor
        internal CommentWriteRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal CommentWriteRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, postNo);
        }

        internal CommentWriteRequest(string galleryId, int postNo, Paragraph paragraph, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, postNo, paragraph);
        }

        internal CommentWriteRequest(string galleryId, int postNo, string nickname, string password, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, postNo, nickname, password);
        }

        internal CommentWriteRequest(string galleryId, int postNo, string nickname, string password, Paragraph paragraph, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, postNo, nickname, password, paragraph);
        }
        #endregion

        #region public override async Task<int> ExecuteAsync()
        public override async Task<int> ExecuteAsync()
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (Content.PostNo == default)
                throw new CSInsideException("'Content.PostNo'의 값을 설정해주세요. ");
            if (Content.PostNo < 1)
                throw new CSInsideException("'Content.PostNo'의 값은 1 이상이어야 합니다.");
            if (Content.Paragraph == null)
                throw new CSInsideException("'Content.Paragraph'의 값을 설정해 주세요.");
            bool isAnonymous = !string.IsNullOrEmpty(Content.Nickname) && !string.IsNullOrEmpty(Content.Password);

            // 변수 초기화
            string galleryId = Content.GalleryId;
            int postNo = Content.PostNo;
            string nickname = null;
            string password = null;
            if (isAnonymous)
            {
                nickname = Content.Nickname;
                password = Content.Password;
            }
            Paragraph paragraph = Content.Paragraph;

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
            if(paragraph is StringParagraph)
            {
                contents.Add(new StringContent((string)paragraph.Content), "comment_memo");
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
        #endregion

        #region public class RequestContent
        public class RequestContent
        {
            public string GalleryId { get; set; }

            public int PostNo { get; set; }
#nullable enable
            public string? Nickname { get; set; }

            public string? Password { get; set; }
#nullable restore
            public Paragraph Paragraph { get; set; }

            internal RequestContent()
            {
                
            }

            internal RequestContent(string galleryId, int postNo)
            {
                GalleryId = galleryId;
                PostNo = postNo;
            }

            internal RequestContent(string galleryId, int postNo, Paragraph paragraph)
            {
                GalleryId = galleryId;
                PostNo = postNo;
                Paragraph = paragraph;
            }

            internal RequestContent(string galleryId, int postNo, string nickname, string password)
            {
                GalleryId = galleryId;
                PostNo = postNo;
                Nickname = nickname;
                Password = password;
            }

            internal RequestContent(string galleryId, int postNo, string nickname, string password, Paragraph paragraph)
            {
                GalleryId = galleryId;
                PostNo = postNo;
                Nickname = nickname;
                Password = password;
                Paragraph = paragraph;
            }
        }
        #endregion
    }
}
