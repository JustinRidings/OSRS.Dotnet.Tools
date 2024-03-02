using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRS.Dotnet.Tools.Types
{
    public class LatestPrices
    {
        [JsonProperty("data")]
        public Dictionary<int, LatestPriceModel>? Data { get; set; }
    }

    public class LatestPriceModel
    {
        public long? High { get; set; }
        public long? HighTime { get; set; }
        public long? Low { get; set; }
        public long? LowTime { get; set; }
    }

    public class LatestMappedPrice
    {
        public Item? Item { get; set; } = default!;
        public LatestPriceModel? LatestPrice { get; set; } = default!;
    }
}
