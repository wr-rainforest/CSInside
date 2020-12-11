
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 디시콘 정보를 나타냅니다.
    /// </summary>
    public class DCCon
    {
        /// <summary>
        /// 디시콘이 포함된 패키지의 고유 식별자입니다.
        /// </summary>
        [JsonProperty("package_idx")]
        public int PackageIndex { get; set; }

        /// <summary>
        /// 디시콘의 고유 식별자입니다.
        /// </summary>
        [JsonProperty("detail_idx")]
        public int DetailIndex { get; set; }

        /// <summary>
        /// 디시콘 제목입니다.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 디시콘 이미지 주소입니다.
        /// </summary>
        [JsonProperty("img")]
        public string ImageUri { get; set; }
    }
}
