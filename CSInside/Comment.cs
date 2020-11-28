using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CSInside
{
    public class Comment : IComparable<Comment>
    {
        [JsonProperty("gallery_id")]
        public string GalleryId { get; set; }

        [JsonProperty("post_no")]
        public int PostNo { get; set; }

        [JsonProperty("comment_no")]
        public int CommentNo { get; set; }

        [JsonProperty("name")]
        private string Name { set => Writer = value; }
        [JsonProperty("writer")]
        public string Writer { get; set; }

        [JsonProperty("user_id")]
        private string userid { set => WriterId = value; }
        [JsonProperty("writer_id")]
        public string WriterId { get; set; }

        [JsonProperty("ipData")]
        private string ip { set => WriterIp = value; }
        [JsonProperty("writer_ip")]
        public string WriterIp { get; set; }

        [JsonProperty("comment_memo")]
        private string CommentMemo { set => Body = value; }
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("date_time")]
        private string DateTime { set => TimeStamp = System.DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", null); }
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("under_step")]
        private bool UnderStep { set => IsReply = value; }
        [JsonProperty("is_reply")]
        public bool IsReply { get; set; }

        [JsonProperty("dccon")]
        private string DCCon { set => DCConUri = value; }
        [JsonProperty("dccon_uri")]
        public string DCConUri { get; set; }

        [JsonProperty("dccon_detail_idx")]
        private int? DCConDetailIdx { set => DCConDetailIndex = value; }
        [JsonProperty("dccon_detail_index")]
        public int? DCConDetailIndex { get; set; }

        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        public int CompareTo([AllowNull] Comment other)
        {
            return CommentNo.CompareTo(other.CommentNo);
        }
    }
}
