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
    public class PostWriteRequest : RequestBase<int>
    {
        public RequestContent Content { get; }

        #region internal ctor
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

        #region public override async Task<int> ExecuteAsync()
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
            var contents = new MultipartFormDataContent();
            keyValuePairs.ToList().ForEach(item => contents.Add(new StringContent(item.Value), item.Key));
            List<ImageParagraph> images = new List<ImageParagraph>();
            paragraphs.ToArray()
                .Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item =>
                {
                    HttpContent content;
                    if (item.Value is StringParagraph)
                    {
                        StringParagraph paragraph = (StringParagraph)item.Value;
                        content = new StringContent($"<div>{HttpUtility.HtmlEncode((string)item.Value.Content)}</div>");
                    }
                    else if (item.Value is ImageParagraph)
                    {
                        ImageParagraph paragraph = (ImageParagraph)item.Value;
                        content = new StringContent($"Dc_App_Img_{images.Count}");
                        images.Add(paragraph);
                    }
                    else
                    {
                        throw new CSInsideException("Paragraph 캐스팅에 실패하였습니다. CSInside.dll에 존재하지 않는 파생 클래스 형식입니다.");
                    }
                    contents.Add(content, $"memo_block[{item.Index}]");
                });
            images
                .Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item => {
                    var byteArrayContent = new ByteArrayContent((byte[])item.Value.Content);
                    byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{item.Value.Extension.Remove(0, 1)}"); 
                    contents.Add(byteArrayContent, $"upload[{item.Index}]", $"image_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{item.Value.Extension}");
                });

            request.Content = contents;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            if ((bool)jObject["result"])
                return (int)jObject["cause"];
            else
                throw new CSInsideException((string)jObject["cause"]);
        }
        #endregion

        #region public class RequestContent
        public class RequestContent
        {
            public string GalleryId { get; set; }

            public string Title { get; set; }
#nullable enable
            public string? Nickname { get; set; }

            public string? Password { get; set; }
#nullable restore
            public ParagraphCollection Paragraphs { get; set; }

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
        }
        #endregion
    }
}
