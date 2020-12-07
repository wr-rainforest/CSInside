using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    internal class PostWriteRequest : RequestBase
    {
        private readonly string galleryId;

        private readonly string nickname;

        private readonly string password;

        private readonly string title;

        private readonly PostContent content;

        internal PostWriteRequest(string galleryId, string nickname, string password, string title, PostContent content, ApiService service) : base(service)
        {
            this.galleryId = galleryId;
            this.nickname = nickname;
            this.password = password;
            this.title = title;
            this.content = content;
        }

        public override async Task ExecuteAsync()
        {
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
            keyValuePairs.Add("name", Uri.EscapeDataString(HttpUtility.HtmlEncode(nickname)));
            keyValuePairs.Add("password", HttpUtility.HtmlEncode(password));
            var content = new MultipartFormDataContent();
            keyValuePairs.ToList().ForEach(item => content.Add(new StringContent(item.Value), item.Key));
            List<byte[]> images = new List<byte[]>();
            this.content.ToArray()
                .Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item =>
                {
                    switch (item.Value.Type)
                    {
                        case ParagraphType.Text:
                            content.Add(new StringContent($"<div>{HttpUtility.HtmlEncode((string)item.Value.Content)}</div>"), $"memo_block[{item.Index}]");
                            break;
                        case ParagraphType.Image:
                            content.Add(new StringContent($"Dc_App_Img_{images.Count}"), $"memo_block[{item.Index}]");
                            images.Add((byte[])item.Value.Content);
                            break;
                        case ParagraphType.DCCon:
                            content.Add(new StringContent((string)item.Value.Content), $"memo_block[{item.Index}]");
                            break;
                    }
                });
            images
                .Select((v, i) => new { Value = v, Index = i })
                .ToList()
                .ForEach(item => {
                    var byteArrayContent = new ByteArrayContent(item.Value);
                    byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{GetImageExtension(item.Value).Remove(0, 1)}"); 
                    content.Add(byteArrayContent, $"upload[{item.Index}]", $"image_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{GetImageExtension(item.Value)}");
                });

            request.Content = content;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            if ((bool)jObject["result"])
                return;
            else
                throw new CSInsideException((string)jObject["cause"]);
        }

        #region GetImageExtension
        private readonly byte[] jpeg = new byte[] { 0xFF, 0xD8 };

        private readonly byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };

        private readonly byte[] gif = new byte[] { 0x47, 0x49, 0x46 };

        private string GetImageExtension(byte[] image)
        {
            if (image.Take(2).SequenceEqual(jpeg))
                return ".jpg";
            else if (image.Take(4).SequenceEqual(png))
                return ".png";
            else if (image.Take(3).SequenceEqual(gif))
                return ".gif";
            throw new Exception("err");
        }
        #endregion
    }
}
