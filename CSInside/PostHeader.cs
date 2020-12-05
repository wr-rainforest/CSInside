using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class PostHeader
    {
        [JsonProperty("gallery_id")]
        public string GalleryId { get; set; }

        [JsonProperty("no")]
        private string No { set => PostNo = int.Parse(value); }
        [JsonProperty("post_no")]
        public int PostNo { get; set; }

        [JsonProperty("subject")]
        private string subject { set => Title = value; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("name")]
        private string Name { set => Writer = value; }
        [JsonProperty("writer")]
        public string Writer { get; set; }

        [JsonProperty("user_id")]
        private string userid { set => WriterId = value; }
        [JsonProperty("writer_id")]
        public string WriterId { get; set; }

        [JsonProperty("ip")]
        private string ip { set => WriterIp = value; }
        [JsonProperty("writer_ip")]
        public string WriterIp { get; set; }

        [JsonProperty("hit")]
        private string hit { set => ViewCount = int.Parse(value); }
        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("recommend")]
        private int recommend { set => UpvoteCount = value; }
        [JsonProperty("upvote_count")]
        public int UpvoteCount { get; set; }

        [JsonProperty("img_icon")]
        private string img_icon { set => ContainsImage = value.Equals("Y"); }
        [JsonProperty("contains_image")]
        public bool ContainsImage { get; set; }

        [JsonProperty("voice_icon")]
        private string voice_icon { set => ContainsVoice = value.Equals("Y"); }
        [JsonProperty("contains_voicd")]
        public bool ContainsVoice { get; set; }

        [JsonProperty("recommend_icon")]
        private string recommend_icon { set => IsRecommendedPost = value.Equals("Y"); }
        [JsonProperty("is_recommended_post")]
        public bool IsRecommendedPost { get; set; }

        [JsonProperty("total_comment")]
        private string _TotalComment { set => CommentCount = int.Parse(value); }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("total_voice")]
        private string total_voice { set => VoiceCommentCount = int.Parse(value); }
        [JsonProperty("voice_comment_count")]
        public int VoiceCommentCount { get; set; }

        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        //[JsonProperty("best_chk")]
        //private string best_chk { set; }

        //[JsonProperty("winnerta_icon")]
        //private string winnerta_icon { set; }

        //[JsonProperty("headtext")]
        //private string headtext { set; }
    }
}
