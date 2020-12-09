using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 게시글
    /// </summary>
    public class Post
    {
        /// <summary>
        /// 갤러리 ID
        /// </summary>
        [JsonProperty("gallery_id")]
        public string GalleryId { get; set; }

        /// <summary>
        /// 게시글 번호
        /// </summary>
        [JsonProperty("no")]
        public int PostNo { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        [JsonProperty("subject")]
        public string Title { get; set; }

        /// <summary>
        /// 작성자 닉네임
        /// </summary>
        [JsonProperty("name")]
        public string Writer { get; set; }

        /// <summary>
        /// 작성자 ID (유동닉일 경우 null)
        /// </summary>
        [JsonProperty("user_id")]
        public string? WriterId { get; set; }

        /// <summary>
        /// 작성자 ID (고닉일 경우 null)
        /// </summary>
        [JsonProperty("ipData")]
        public string? WriterIp { get; set; }

        /// <summary>
        /// 조회수
        /// </summary>
        [JsonProperty("hit")]
        public int ViewCount { get; set; }

        /// <summary>
        /// 추천수
        /// </summary>
        [JsonProperty("upvote_count")]
        public int UpvoteCount { get; set; }

        /// <summary>
        /// 고닉 추천수
        /// </summary>
        [JsonProperty("member_upvote_count")]
        public int MemberUpvoteCount { get; set; }

        /// <summary>
        /// 비추천수
        /// </summary>
        [JsonProperty("downvote_count")]
        public int DownvoteCount { get; set; }

        /// <summary>
        /// 게시글 본문
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// 댓글 개수
        /// </summary>
        [JsonProperty("total_comment")]
        public int CommentCount { get; set; }

        [JsonProperty("isNotice")]
        private string isNotice { get => IsNotice ? "Y" : "N"; set => IsNotice = value.Equals("Y"); }
        /// <summary>
        /// 공지 여부
        /// </summary>
        public bool IsNotice { get; set; }

        [JsonProperty("date_time")]
        private string DateTime
        {
            get => TimeStamp.ToString("yyyy.MM.dd HH:mm");
            set => TimeStamp = System.DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", null);
        }
        /// <summary>
        /// 게시글 작성 시간 (yyyy.MM.dd HH:mm)
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("write_type")]
        public string WriteType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("category")]
        public int Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
