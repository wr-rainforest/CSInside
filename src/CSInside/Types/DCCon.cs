
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    public class DCCon
    {
        [JsonProperty("package_idx")]
        public int PackageIndex { get; set; }

        [JsonProperty("detail_idx")]
        public string DetailIndex { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("img")]
        public string ImageUri { get; set; }
    }
}
