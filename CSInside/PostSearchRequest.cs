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
    public class PostSearchRequest : RequestBase<PostHeader[]>
    {
        private readonly string galleryId;

        private readonly string keyword;

        private readonly string s_type;
#nullable enable
        private int? ser_pos = null;
#nullable restore
        private int position;

        private int pageCount;

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
            if (ser_pos > 0)
            {
                return null;
            }
            List<PostHeader> postHeaders = new List<PostHeader>();
            string app_id = AuthTokenProvider.GetAccessToken();
            string hash = Uri.EscapeUriString($"http://app.dcinside.com/api/gall_list_new.php?id={galleryId}&page={position}&app_id={app_id}&s_type={s_type}&serVal={keyword}{(ser_pos == null ? string.Empty : $"&ser_pos={ser_pos}")}").ToBase64String(Encoding.ASCII);
            string uri = $"http://app.dcinside.com/api/redirect.php?hash={hash}";
            string responseString;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("Connection", "Keep-Alive");
                var response = await Client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    throw new CSInsideException($"API 서버에서 Internal Server Error를 반환하였습니다. 인증 토큰이 만료되었거나 올바르지 않은 인증 토큰일 수 있습니다.");
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CSInsideException))
                    throw;
                throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
            }
            if (string.IsNullOrEmpty(responseString))
            {
                throw new CSInsideException($"예기치 않은 오류: 서버에서 빈 문자열을 반환하였습니다.");
            }

            //예외처리
            JToken jToken;
            try
            {
                jToken = JToken.Parse(responseString);
            }
            catch(Exception e)
            {
                throw new CSInsideException($"예기치 않은 오류: Json 파싱에 실패하였습니다.", e);
            }
            JObject jObject = jToken is JObject ? jToken as JObject : (jToken as JArray)[0] as JObject;

            //반환값 처리
            var list = jObject["gall_list"].ToObject<List<PostHeader>>();
            list.ForEach(item => { item.GalleryId = galleryId; });
            postHeaders.AddRange(list);
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
            return postHeaders.ToArray();
        }
    }
    public enum SearchType
    {
        All = 0,
        Title = 1,
        Content = 2,
        Writer = 3,
        TitleContent = 4
    }
}
