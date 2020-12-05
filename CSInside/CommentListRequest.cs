using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;
using System.Net.Http;

namespace CSInside
{
    public class CommentListRequest : RequestBase<Comment[]>
    {
        private readonly string galleryId;

        private readonly int postNo;

        public string GalleryId { get => galleryId; }

        public int PostNo { get => postNo; }

        /// <exception cref="ArgumentNullException"></exception>
        internal CommentListRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            // 매개변수 검사
            if (galleryId is null)
                throw new ArgumentNullException(nameof(galleryId));

            // 필드 초기화
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
                // HTTP 요청 생성
                string app_id = base.AuthTokenProvider.GetAccessToken();
                string hash = $"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={postNo}&re_page={i}&app_id={app_id}".ToBase64String(Encoding.ASCII);
                string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}");
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

                // 반환값 처리
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
