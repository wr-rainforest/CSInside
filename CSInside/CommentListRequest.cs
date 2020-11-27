using CSInside.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CSInside
{
    public class CommentListRequest : RequestBase<Comment[]>
    {
        private string galleryId;

        private int postNo;

        public CommentListRequest(string galleryId, int postNo, HttpClient client, IAuthTokenProvider authTokenProvider) : base(client, authTokenProvider)
        {
            this.galleryId = galleryId;
            this.postNo = postNo;
        }

        public override async Task<Comment[]> Execute()
        {
            string appid = AuthTokenProvider.GetAppId();
            List<Comment> comments = new List<Comment>();
            for (int i = 1, totalPage = 1; i <= totalPage; i++)
            {
                string hash = $"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={postNo}&re_page={i}&app_id={appid}".ToBase64String(Encoding.ASCII);
                string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}");
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await Client.SendAsync(request);
                string jsonString = await response.Content.ReadAsStringAsync();
                JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
                if (jObject.ContainsKey("result"))
                {
                    throw new ApiRequestException(1000, jObject["cause"].ToString());
                }
                totalPage = (int)jObject["total_page"];
                comments.AddRange(jObject["comment_list"].ToObject<Comment[]>());
            }
            comments.ForEach(item => { item.GalleryId = galleryId; item.PostNo = postNo; });
            return comments.Distinct().ToArray();
        }
    }
}
