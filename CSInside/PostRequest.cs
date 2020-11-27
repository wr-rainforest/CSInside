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
    public class PostRequest : RequestBase<Post>
    {
        private string galleryId;

        private int postNo;

        internal PostRequest(string galleryId, int postNo, ApiService service) : base(service)
        {
            this.galleryId = galleryId;
            this.postNo = postNo;
        }

        public override async Task<Post> Execute()
        {
            string appid = AuthTokenProvider.GetAppId();
            string hash = $"http://app.dcinside.com/api/gall_view_new.php?id={galleryId}&no={postNo}&app_id={appid}".ToBase64String(Encoding.ASCII);
            string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}&app_id={appid}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Client.SendAsync(request);
            var jArray = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync());
            var jObject = jArray[0].ToObject<JObject>();
            if (jObject.ContainsKey("result") && (string)jObject["cause"] == "글없음")
            {
                throw new ApiRequestException(500, "게시글이 존재하지 않거나 삭제되었습니다.");
            }
            Post post = JsonConvert.DeserializeObject<Post>(jObject["view_info"].ToString());
            post.GalleryId = galleryId;
            post.Body = HttpUtility.HtmlDecode(jObject["view_main"]["memo"].ToString());
            post.UpvoteCount = int.Parse(jObject["view_main"]["recommend"].ToString());
            post.MemberUpvoteCount = int.Parse(jObject["view_main"]["recommend_member"].ToString());
            post.DownvoteCount = int.Parse(jObject["view_main"]["nonrecommend"].ToString());
            return post;
        }
    }
}
