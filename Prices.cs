using Newtonsoft.Json;
using OSRS.Dotnet.Tools.Types;
using System.Collections.Concurrent;
using System.Net;

namespace OSRS.Dotnet.Tools
{
    public class Prices
    {
        private const string _pricesUri = @"http://prices.runescape.wiki/";
        private const string _apiEndpoint = "api/v1/osrs";
        private const string _mappingEndpoint = $"{_apiEndpoint}/mapping";
        private const string _latestEndpoint = $"{_apiEndpoint}/latest";
        private const string _fiveMinEndpoint = $"{_apiEndpoint}/5m";
        private const string _oneHrEndpoint = $"{_apiEndpoint}/1h";

        private static SocketsHttpHandler _httpHandler { get; set; } = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2)
        };

        private static HttpClient _httpClient { get; set; } = new HttpClient(_httpHandler)
        {
            BaseAddress = new Uri(_pricesUri)
        };


        public static async Task<List<Item>> GetItemMappingsAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            List<Item> results = new List<Item>();

            using (var response = await _httpClient.GetAsync(_mappingEndpoint))
            {
                if(response != null && response.IsSuccessStatusCode)
                {
                    var items = JsonConvert.DeserializeObject<List<Item>>(await response.Content.ReadAsStringAsync());
                    results = items ?? new List<Item>();
                }
            }

            return results;
        }

        public static async Task<LatestPrices> GetLatestPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            LatestPrices results = default!;

            using (var response = await _httpClient.GetAsync(_latestEndpoint))
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<LatestPrices>(await response.Content.ReadAsStringAsync());
                    results = items ?? new LatestPrices();
                }
            }

            return results;
        }

        public static async Task<FiveMinPrices> GetFiveMinPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            FiveMinPrices results = default!;

            using (var response = await _httpClient.GetAsync(_fiveMinEndpoint))
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<FiveMinPrices>(await response.Content.ReadAsStringAsync());
                    results = items ?? new FiveMinPrices();
                }
            }

            return results;
        }

        public static async Task<OneHrPrices> GetOneHrPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            OneHrPrices results = default!;

            using (var response = await _httpClient.GetAsync(_oneHrEndpoint))
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<OneHrPrices>(await response.Content.ReadAsStringAsync());
                    results = items ?? new OneHrPrices();
                }
            }

            return results;
        }

        public static async Task<List<OneHrMappedPrice>> GetOneHrMappedPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            var mappedPriceList = new List<OneHrMappedPrice>();
            var itemMappings = await GetItemMappingsAsync(discordUsername);
            var oneHrPrices = await GetOneHrPricesAsync(discordUsername);
            ConcurrentBag<OneHrMappedPrice> oneHrMappedPrices = new ConcurrentBag<OneHrMappedPrice>();

            if (oneHrPrices.Data != null)
            {
                Parallel.ForEach(oneHrPrices.Data, p =>
                {
                    oneHrMappedPrices.Add(new OneHrMappedPrice()
                    {
                        Item = itemMappings.Where(e => e.Id == p.Key).FirstOrDefault() ?? default!,
                        OneHrPrice = p.Value,
                    });
                });
            }

            return oneHrMappedPrices.ToList();
        }

        public static async Task<List<FiveMinMappedPrice>> GetFiveMinMappedPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            var mappedPriceList = new List<FiveMinMappedPrice>();
            var itemMappings = await GetItemMappingsAsync(discordUsername);
            var fiveMinPrices = await GetFiveMinPricesAsync(discordUsername);
            ConcurrentBag<FiveMinMappedPrice> fiveMinMappedPrices = new ConcurrentBag<FiveMinMappedPrice>();

            if (fiveMinPrices.Data != null)
            {
                Parallel.ForEach(fiveMinPrices.Data, p =>
                {
                    fiveMinMappedPrices.Add(new FiveMinMappedPrice()
                    {
                        Item = itemMappings.Where(e => e.Id == p.Key).FirstOrDefault() ?? default!,
                        FiveMinPrice = p.Value,
                    });
                });
            }

            return fiveMinMappedPrices.ToList();
        }

        public static async Task<List<LatestMappedPrice>> GetLatestMappedPricesAsync(string discordUsername)
        {
            SetUserAgent(discordUsername);

            var mappedPriceList = new List<LatestMappedPrice>();
            var itemMappings = await GetItemMappingsAsync(discordUsername);
            var latestPrices = await GetLatestPricesAsync(discordUsername);
            ConcurrentBag<LatestMappedPrice> latestMappedPrices = new ConcurrentBag<LatestMappedPrice>();
            
            if (latestPrices.Data != null)
            {
                Parallel.ForEach(latestPrices.Data, p =>
                {
                    latestMappedPrices.Add(new LatestMappedPrice()
                    {
                        Item = itemMappings.Where(e => e.Id == p.Key).FirstOrDefault() ?? default!,
                        LatestPrice = p.Value,
                    });
                });
            }

            return latestMappedPrices.ToList();
        }



        private static void SetUserAgent(string discordUsername)
        {
            string userAgent = $"volume_tracker - discord/{discordUsername}";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public static DateTime? GetDateFromUnixTime(long? unixTimestamp)
        {
            var normalizedStamp = unixTimestamp ?? 0;
            return DateTimeOffset.FromUnixTimeSeconds(normalizedStamp).DateTime;
        }


    }
}
