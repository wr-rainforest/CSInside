using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class Article
    {
        [JsonProperty("gallery_id")]
        public string GalleryId { get; set; }

        [JsonProperty("no")]
        private string No { set => ArticleNo = int.Parse(value); }
        [JsonProperty("article_no")]
        public int ArticleNo { get; set; }

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

        [JsonProperty("upvote_count")]
        public int UpvoteCount { get; set; }

        [JsonProperty("member_upvote_count")]
        public int MemberUpvoteCount { get; set; }

        [JsonProperty("downvote_count")]
        public int DownvoteCount { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("total_comment")]
        private string _TotalComment { set => CommentCount = int.Parse(value); }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("isNotice")]
        private string isNotice { set => IsNotice = value.Equals("Y"); }
        [JsonProperty("is_notice")]
        public bool IsNotice { get; set; }

        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        [JsonProperty("write_type")]
        public string WriteType { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }
    }
}
