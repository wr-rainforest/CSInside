using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSInside.Extensions;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace CSInside
{
    public class GallerySearchRequest : RequestBase<GallerySearchResult>
    {
        private readonly string keyword;

        public string KeyWord { get => keyword; }

        /// <exception cref="ArgumentNullException"></exception>
        internal GallerySearchRequest(string keyword, ApiService service) : base(service)
        {
            this.keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public async override Task<GallerySearchResult> ExecuteAsync()
        {
            // HTTP 요청 생성
            string app_id = base.AuthTokenProvider.GetAccessToken();
            string uri =  "http://app.dcinside.com/api/_total_search.php";
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("keyword", keyword);
            keyValuePairs.Add("app_id", app_id);
            keyValuePairs.Add("search_type", "gall_name");
            var content = new MultipartFormDataContent();
            keyValuePairs.ToArray().ToList().ForEach(item => content.Add(new StringContent(item.Value), item.Key));
            request.Content = content;

            // 전송
            var task = base.GetResponseAsync(request);

            // 응답 수신
            JObject jObject = await task;

            // 예외처리
            if (!jObject.ContainsKey("main_gall"))
                throw new CSInsideException($"예기치 않은 오류: {jObject.ToString(Formatting.None)}");

            // 반환값 처리
            GallerySearchResult result = JsonConvert.DeserializeObject<GallerySearchResult>(jObject.ToString());
            result.MainGalleries.ToList().ForEach(item => item.IsMinor = false);
            result.MainRecommendGalleries.ToList().ForEach(item => item.IsMinor = false);
            result.MinorGalleries.ToList().ForEach(item => item.IsMinor = true);
            result.MinorRecommendGalleries.ToList().ForEach(item => item.IsMinor = true);
            return result;
        }
    }
}
