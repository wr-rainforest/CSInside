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

        private int articleNo;

        public CommentListRequest(string galleryId, int articleNo, HttpClient client, IAuthTokenProvider authTokenProvider) : base(client, authTokenProvider)
        {
            this.galleryId = galleryId;
            this.articleNo = articleNo;
        }

        public override async Task<Comment[]> Execute()
        {
            string appid = AuthTokenProvider.GetAppId();
            List<Comment> comments = new List<Comment>();
            for (int i = 1, totalPage = 1; i <= totalPage; i++)
            {
                string hash = $"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={articleNo}&re_page={i}&app_id={appid}".ToBase64String(Encoding.ASCII);
                string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}");
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await Client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var jArray = JsonConvert.DeserializeObject<JArray>(responseString);
                var jObject = jArray[0].ToObject<JObject>();
                if (jObject.ContainsKey("result"))
                {
                    throw new ApiRequestException(1000, jObject["cause"].ToString());
                }
                totalPage = (int)jObject["total_page"];
                comments.AddRange(jObject["comment_list"].ToObject<Comment[]>());
            }
            comments.ForEach(item => { item.GalleryId = galleryId; item.ArticleNo = articleNo; });
            return comments.Distinct().ToArray();
        }

        public async Task<Comment[]> Execute(int pageNo)
        {
            throw new NotImplementedException();
            string appid = AuthTokenProvider.GetAppId();
            string hash = $"http://app.dcinside.com/api/comment_new.php?id={galleryId}&no={articleNo}&re_page={pageNo}&app_id={appid}".ToBase64String(Encoding.ASCII);
            string uri = Uri.EscapeUriString($"http://app.dcinside.com/api/redirect.php?hash={hash}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Client.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync(); Console.WriteLine(responseString);
            JArray jArray;
            try 
            {
                //
                //[{"total_comment":"0","total_page":"1","re_page":"101","comment_list":[]}]
                jArray = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                //
                //{"message":"Whoops, looks like something went wrong","status":500}
                JObject errObj = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                return new Comment[0];
            }
            var jObject = jArray[0].ToObject<JObject>();
            if(pageNo > (int)jObject["total_page"])
            {
                return new Comment[0];
            }
            var comments = jObject["comment_list"].ToObject<List<Comment>>();
            comments.ForEach(item => { item.GalleryId = galleryId; item.ArticleNo = articleNo; });
            return comments.ToArray();
        }
    }
}
