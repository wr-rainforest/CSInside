using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CSInside
{
    /// <summary>
    /// 디시인사이드 갤러리
    /// </summary>
    public class Gallery
    {
        /// <summary>
        /// 갤러리 Id를 가져오거나 설정합니다.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 갤러리 이름을 가져오거나 설정합니다
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 마이너 갤러리 여부를 나타내는 값을 가져오거나 설정합니다.
        /// </summary>
        [JsonProperty("is_minor")]
        public bool IsMinor { get; set; }
    }
}
