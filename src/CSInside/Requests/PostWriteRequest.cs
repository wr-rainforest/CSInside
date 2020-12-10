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
    /// 게시글 작성 API 요청을 나타냅니다.
    /// </summary>
    public class PostWriteRequest : RequestBase<int>
    {
        /// <summary>
        /// 요청 변수를 가져옵니다.
        /// </summary>
        public RequestContent Content { get; }

        #region ctor
        internal PostWriteRequest(ApiService service) : base(service)
        {
            Content = new RequestContent();
        }

        internal PostWriteRequest(string galleryId, string title, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, title);
        }

        internal PostWriteRequest(string galleryId, string title, ParagraphCollection paragraphs, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, title, paragraphs);
        }

        internal PostWriteRequest(string galleryId, string title, string nickname, string password, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, title, nickname, password);
        }

        internal PostWriteRequest(string galleryId, string title, string nickname, string password, ParagraphCollection paragraphs, ApiService service) : base(service)
        {
            Content = new RequestContent(galleryId, title, nickname, password, paragraphs);
        }
        #endregion

        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns>작성한 게시글의 번호입니다.</returns>
        /// <exception cref="CSInsideException"></exception>
        public override async Task<int> ExecuteAsync()
        {
            // Content값 검증
            if (string.IsNullOrEmpty(Content.GalleryId))
                throw new CSInsideException("'Content.GalleryId'의 값을 설정해 주세요.");
            if (string.IsNullOrEmpty(Content.Title))
                throw new CSInsideException("'Content.Title'의 값을 설정해 주세요.");
            if (Content.Paragraphs.Count == 0)
                throw new CSInsideException("'Content.Paragraphs' == 0");
            bool isAnonymous = !string.IsNullOrEmpty(Content.Nickname) && !string.IsNullOrEmpty(Content.Password);

            // 변수 초기화
            string galleryId = Content.GalleryId;
            string title = Content.Title;
            string nickname = null;
            string password = null;
            if (isAnonymous)
            {
                nickname = Content.Nickname;
                password = Content.Password;
            }
            ParagraphCollection paragraphs = Content.Paragraphs;

            // HTTP 요청 생성
            string appId = base.AuthTokenProvider.GetAccessToken();
            string client_token = base.AuthTokenProvider.GetClientToken();
            string uri = "http://upload.dcinside.com/_app_write_api.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("id", galleryId);
            keyValuePairs.Add("app_id", appId);
            keyValuePairs.Add("mode", "write");
            keyValuePairs.Add("client_token", client_token);
            keyValuePairs.Add("subject", Uri.EscapeDataString(HttpUtility.HtmlEncode(title)));
            if (isAnonymous)
            {
                keyValuePairs.Add("name", Uri.EscapeDataString(HttpUtility.HtmlEncode(nickname)));
                keyValuePairs.Add("password", HttpUtility.HtmlEncode(password));
            }
            else
            {
                string user_id = AuthTokenProvider.GetUserToken();
                keyValuePairs.Add("user_id", user_id);
            }
            var multipartFormDataContent = new MultipartFormDataContent();
            keyValuePairs.ToList().ForEach(item => multipartFormDataContent.Add(new StringContent(item.Value), item.Key));

            List<ImageParagraph> imageParagraphs = new List<ImageParagraph>();
            paragraphs.ToArray()
                .Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item =>
                {
                    HttpContent content;
                    if (item.Value is StringParagraph)
                    {
                        content = item.Value.GetHttpContent();
                    }
                    else if (item.Value is ImageParagraph imgpar)
                    {
                        content = new StringContent($"Dc_App_Img_{imageParagraphs.Count}");
                        imageParagraphs.Add(imgpar);
                    }
                    else if (item.Value is DCConParagraph dcconpar)
                    {
                        var stringContent = new StringContent(dcconpar.DCCon.DetailIndex.ToString());
                        multipartFormDataContent.Add(stringContent, $"detail_idx[{item.Index}]");
                        content = item.Value.GetHttpContent();
                    }
                    else
                    {
                        throw new CSInsideException("Paragraph 캐스팅에 실패하였습니다. CSInside.dll에 존재하지 않는 파생 클래스 형식입니다.");
                    }
                    multipartFormDataContent.Add(content, $"memo_block[{item.Index}]");
                });
            imageParagraphs.Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item => {
                    var byteArrayContent = item.Value.GetHttpContent();
                    byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{item.Value.Extension.Remove(0, 1)}"); 
                    multipartFormDataContent.Add(byteArrayContent, $"upload[{item.Index}]", $"image_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{item.Value.Extension}");
                });
            request.Content = multipartFormDataContent;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            if ((bool)jObject["result"])
                return (int)jObject["cause"];
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
            /// 게시글 제목입니다. (필수 요청 변수)
            /// </summary>
            public string Title { get; set; }
#nullable enable
            /// <summary>
            /// 유동으로 게시글을 작성할 시 사용할 닉네임입니다. (선택 요청 변수)
            /// </summary>
            public string? Nickname { get; set; }

            /// <summary>
            /// 유동으로 게시글을 작성할 시 사용할 비밀번호입니다. (선택 요청 변수)
            /// </summary>
            public string? Password { get; set; }
#nullable restore
            /// <summary>
            /// 문단 컬렉션입니다. (필수 요청 변수)
            /// </summary>
            public ParagraphCollection Paragraphs { get; set; }
            #region ctor
            internal RequestContent()
            {
                Paragraphs = new ParagraphCollection();
            }

            internal RequestContent(string galleryId, string title)
            {
                GalleryId = galleryId;
                Title = title;
                Paragraphs = new ParagraphCollection();
            }

            internal RequestContent(string galleryId, string title, ParagraphCollection paragraphs)
            {
                GalleryId = galleryId;
                Title = title;
                Paragraphs = paragraphs;
            }

            internal RequestContent(string galleryId, string title, string nickname, string password)
            {
                GalleryId = galleryId;
                Title = title;
                Nickname = nickname;
                Password = password;
                Paragraphs = new ParagraphCollection();
            }

            internal RequestContent(string galleryId, string title, string nickname, string password, ParagraphCollection paragraphs)
            {
                GalleryId = galleryId;
                Title = title;
                Nickname = nickname;
                Password = password;
                Paragraphs = paragraphs;
            }
            #endregion
        }
    }
}
