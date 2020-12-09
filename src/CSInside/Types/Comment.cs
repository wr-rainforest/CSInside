using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 댓글
    /// </summary>
    public class Comment : IComparable<Comment>
    {
        /// <summary>
        /// 갤러리 ID
        /// </summary>
        public string GalleryId { get; set; }

        /// <summary>
        /// 게시글 번호
        /// </summary>
        public int PostNo { get; set; }

        /// <summary>
        /// 댓글 번호
        /// </summary>
        [JsonProperty("comment_no")]
        public int CommentNo { get; set; }

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
        /// 댓글 본문
        /// </summary>
        [JsonProperty("comment_memo")]
        public string Body { get; set; }

        [JsonProperty("date_time")]
        private string DateTime 
        {
            get => TimeStamp.ToString("yyyy.MM.dd HH:mm");
            set => TimeStamp = System.DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", null);
        }
        /// <summary>
        /// 댓글 작성 시간 (yyyy.MM.dd HH:mm)
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 답글 여부
        /// </summary>
        [JsonProperty("under_step")]
        public bool IsReply { get; set; }

        /// <summary>
        /// 디시콘 uri
        /// </summary>
        [JsonProperty("dccon")]
        public string? DCConUri { get; set; }

        /// <summary>
        /// 디시콘 
        /// </summary>
        [JsonProperty("dccon_detail_idx")]
        public int? DCConDetailIndex { get; set; }

        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        public int CompareTo([AllowNull] Comment other)
        {
            return CommentNo.CompareTo(other.CommentNo);
        }
    }
}
