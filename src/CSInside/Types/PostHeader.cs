using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class PostHeader
    {
        /// <summary>
        /// 갤러리 ID
        /// </summary>
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
        [JsonProperty("recommend")]
        public int UpvoteCount { get; set; }

        [JsonProperty("img_icon")]
        private string img_icon { get => ContainsImage ? "Y" : "N"; set => ContainsImage = value.Equals("Y"); }
        /// <summary>
        /// 이미지 포함 여부
        /// </summary>
        public bool ContainsImage { get; set; }

        [JsonProperty("voice_icon")]
        private string voice_icon { get => ContainsVoice ? "Y" : "N"; set => ContainsVoice = value.Equals("Y"); }
        /// <summary>
        /// 보이스 포함 여부
        /// </summary>
        public bool ContainsVoice { get; set; }

        [JsonProperty("recommend_icon")]
        private string recommend_icon { get=> IsRecommendedPost ? "Y" : "N"; set => IsRecommendedPost = value.Equals("Y"); }
        /// <summary>
        /// 개념글 여부
        /// </summary>
        public bool IsRecommendedPost { get; set; }

        /// <summary>
        /// 댓글 개수
        /// </summary>
        [JsonProperty("total_comment")]
        public int CommentCount { get; set; }

        /// <summary>
        /// 보이스플 개수
        /// </summary>
        [JsonProperty("total_voice")]
        public int VoiceCommentCount { get; set; }

        [JsonProperty("member_icon")]
        public string MemberIcon { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        /// <summary>
        /// 게시글 작성 시간. 형식 일정하지 않음
        /// </summary>
        [JsonProperty("date_time")]
        public string TimeStamp { get; set; }

        //[JsonProperty("best_chk")]
        //private string best_chk { set; }

        //[JsonProperty("winnerta_icon")]
        //private string winnerta_icon { set; }

        //[JsonProperty("headtext")]
        //private string headtext { set; }
    }
}
