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
#nullable enable
    internal class CommentReader : ReaderBase<Comment[]?>
#nullable restore
    {
        private readonly string galleryId;

        private readonly int postNo;

        private int currentPage;

        private int pageCount;

        private List<int> fetchedComments;

        public override int Position { get => currentPage - 1; }

        public override int Count { get => pageCount; }

        internal CommentReader(string galleryId, int postNo, ApiService service) : base(service)
        {
            this.galleryId = galleryId;
            this.postNo = postNo;
            this.currentPage = 1;
            this.pageCount = 1;
            this.fetchedComments = new List<int>();
        }

#nullable enable
        public override async Task<Comment[]?> ReadAsync()
#nullable restore
        {
            // 상태 체크
            if (pageCount >= currentPage + 1)
            {
                currentPage++;
            }

            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string hash = Uri.EscapeUriString($"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={postNo}&re_page={currentPage}&app_id={app_id}").ToBase64String(Encoding.ASCII);
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
            if (currentPage > pageCount)
                return null;
            var comments = jObject["comment_list"].ToObject<List<Comment>>();
            if (comments.Count == 0)
                // 존재하지 않는 게시글 / 페이지
                return null;
            comments = comments.Where(item => !fetchedComments.Contains(item.CommentNo)).ToList();
            if (comments.Count == 0)
                // 해당 페이지의 모든 댓글을 이미 불러왔을 경우
                return new Comment[0];

            comments.ForEach(item =>
            {
                item.GalleryId = galleryId;
                item.PostNo = postNo;
                fetchedComments.Add(item.CommentNo);
            });
            return comments.ToArray();
        }
    }
}
