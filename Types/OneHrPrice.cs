using Newtonsoft.Json;

namespace OSRS.Dotnet.Tools.Types
{
    public class OneHrPrices
    {
        [JsonProperty("data")]
        public Dictionary<int, OneHrPriceModel>? Data { get; set; }
    }

    public class OneHrPriceModel
    {
        public long? AvgHighPrice { get; set; }
        public long? HighPriceVolume { get; set; }
        public long? AvgLowPrice { get; set; }
        public long? LowPriceVolume { get; set; }
    }

    public class OneHrMappedPrice
    {
        public Item? Item { get; set; } = default!;
        public OneHrPriceModel? OneHrPrice { get; set; } = default!;
    }
}
