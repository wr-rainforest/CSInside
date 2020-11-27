using CSInside.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CSInside
{
    public class ArticleRequest : RequestBase<Article>
    {
        private string galleryId;

        private int articleNo;

        internal ArticleRequest(string galleryId, int articleNo, HttpClient client, IAuthTokenProvider authTokenProvider) : base(client, authTokenProvider)
        {
            this.galleryId = galleryId;
            this.articleNo = articleNo;
        }

        public override async Task<Article> Execute()
        {
            string appid = AuthTokenProvider.GetAppId();
            string hash = $"http://app.dcinside.com/api/gall_view_new.php?id={galleryId}&no={articleNo}&app_id={appid}".ToBase64String(Encoding.ASCII);
            string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}&app_id={appid}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Client.SendAsync(request);
            var jArray = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync());
            var jObject = jArray[0].ToObject<JObject>();
            if (jObject.ContainsKey("result") && (string)jObject["cause"] == "글없음")
            {
                throw new ApiRequestException(500, "게시글이 존재하지 않거나 삭제되었습니다.");
            }
            Article article = JsonConvert.DeserializeObject<Article>(jObject["view_info"].ToString());
            article.GalleryId = galleryId;
            article.Body = HttpUtility.HtmlDecode(jObject["view_main"]["memo"].ToString());
            article.UpvoteCount = int.Parse(jObject["view_main"]["recommend"].ToString());
            article.MemberUpvoteCount = int.Parse(jObject["view_main"]["recommend_member"].ToString());
            article.DownvoteCount = int.Parse(jObject["view_main"]["nonrecommend"].ToString());
            return article;
        }
    }
}
