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

        /// <exception cref="ArgumentNullException"></exception>
        public GallerySearchRequest(string keyword, ApiService service) : base(service)
        {
            this.keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        }

        public async override Task<GallerySearchResult> ExecuteAsync()
        {
            //HTTP 요청
            string app_id = AuthTokenProvider.GetAccessToken();
            string uri =  "http://app.dcinside.com/api/_total_search.php";
            string responseString;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("keyword", keyword);
                keyValuePairs.Add("app_id", app_id);
                keyValuePairs.Add("search_type", "gall_name");
                var content = new MultipartFormDataContent();
                keyValuePairs.ToArray().ToList().ForEach(item => content.Add(new StringContent(item.Value), item.Key));
                request.Content = content;
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
            JObject jObject = JToken.Parse(responseString) is JObject ? JToken.Parse(responseString) as JObject : (JToken.Parse(responseString) as JArray)[0] as JObject;
            if (!jObject.ContainsKey("main_gall"))
                throw new CSInsideException($"예기치 않은 오류: {jObject.ToString(Formatting.None)}");

            //반환값 처리
            GallerySearchResult result = JsonConvert.DeserializeObject<GallerySearchResult>(jObject.ToString());
            result.MainGalleries.ToList().ForEach(item => item.IsMinor = false);
            result.MainRecommendGalleries.ToList().ForEach(item => item.IsMinor = false);
            result.MinorGalleries.ToList().ForEach(item => item.IsMinor = true);
            result.MinorRecommendGalleries.ToList().ForEach(item => item.IsMinor = false);
            return result;
        }
    }
}
