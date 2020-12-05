using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    /// <summary>
    /// 갤러리 검색 결과
    /// </summary>
    public class GallerySearchResult
    {
        /// <summary>
        /// 갤러리 검색 결과
        /// </summary>
        [JsonProperty("main_gall")]
        public Gallery[] MainGalleries { get; set; }

        /// <summary>
        /// 마이너 갤러리 검색 결과
        /// </summary>
        [JsonProperty("minor_gall")]
        public Gallery[] MinorGalleries { get; set; }

        /// <summary>
        /// 추천 갤러리 검색 결과
        /// </summary>
        [JsonProperty("main_recomm_gall")]
        public Gallery[] MainRecommendGalleries { get; set; }

        /// <summary>
        /// 추천 마이너 갤러리 검색 결과
        /// </summary>
        [JsonProperty("minor_recomm_gall")]
        public Gallery[] MinorRecommendGalleries { get; set; }
    }
}
