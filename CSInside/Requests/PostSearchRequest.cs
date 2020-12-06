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
    internal class PostSearchRequest : RequestBase<PostHeader[]>
    {
        private readonly string galleryId;

        private readonly string keyword;

        private readonly string s_type;

#nullable enable
        private int? ser_pos = null;
#nullable restore
        private int position;

        private int pageCount;

        public string GalleryId { get => galleryId; }

        public string Keyword { get => keyword; }

        internal PostSearchRequest(string galleryId, string keyword, SearchType searchType, ApiService service) : base(service)
        {
            this.galleryId = galleryId;
            this.keyword = keyword;
            s_type = searchType switch
            {
                SearchType.All => "all",
                SearchType.Title => "subject",
                SearchType.Content => "memo",
                SearchType.Writer => "name",
                SearchType.TitleContent => "subject_m",
                _ => throw new NotImplementedException("enum")
            };
            position = 1;
            pageCount = 1;
        }
#nullable enable
        public override async Task<PostHeader[]?> ExecuteAsync()
#nullable restore
        {
            // 상태 체크
            if (ser_pos > 0)
                return null;

            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string hash = Uri.EscapeUriString($"http://app.dcinside.com/api/gall_list_new.php?id={galleryId}&page={position}&app_id={app_id}&s_type={s_type}&serVal={keyword}{(ser_pos == null ? string.Empty : $"&ser_pos={ser_pos}")}").ToBase64String(Encoding.ASCII);
            string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 상태 정의
            pageCount = (int)jObject["gall_info"][0]["ser_total_page"];
            if (pageCount >= position + 1)
            {
                position++;
            }
            else
            {
                position = 1;
                pageCount = 1;
                ser_pos = (int)jObject["gall_info"][0]["ser_pos"];
            }

            // 반환값 처리
            var postHeaders = jObject["gall_list"].ToObject<List<PostHeader>>();
            postHeaders.ForEach(item => { item.GalleryId = galleryId; });
            return postHeaders.ToArray();
        }
    }
}
