using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRS.Dotnet.Tools.Types
{
    public class FiveMinPrices
    {
        [JsonProperty("data")]
        public Dictionary<int, FiveMinPriceModel>? Data { get; set; }
    }

    public class FiveMinPriceModel
    {
        public long? AvgHighPrice { get; set; }
        public long? HighPriceVolume { get; set; }
        public long? AvgLowPrice { get; set; }
        public long? LowPriceVolume { get; set; }
    }

    public class FiveMinMappedPrice
    {
        public Item? Item { get; set; } = default!;
        public FiveMinPriceModel? FiveMinPrice { get; set; } = default!;
    }
}
