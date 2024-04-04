using Newtonsoft.Json;
using OSRS.Dotnet.Tools.Types;
using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Text;

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
                if (response != null && response.IsSuccessStatusCode)
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

        public static async Task<bool> AppendLatestPricesToCsv(string discordUsername, string csvFilePath)
        {
            SetUserAgent(discordUsername);
            var latestPrices = await GetLatestMappedPricesAsync(discordUsername);

            DataTable dt = new DataTable();
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("ItemId", typeof(string));
            dt.Columns.Add("BuyLimit", typeof(long));
            dt.Columns.Add("High", typeof(long));
            dt.Columns.Add("HighTimeUTC", typeof(DateTime));
            dt.Columns.Add("Average", typeof(long));
            dt.Columns.Add("CurrentAvgTimeUTC", typeof(DateTime));
            dt.Columns.Add("Low", typeof(long));
            dt.Columns.Add("LowTimeUTC", typeof(DateTime));

            foreach (var item in latestPrices)
            {
                if (item != null && item.Item != null && item.Item.Name != null)
                {
                    DataRow row = dt.NewRow();

                    row["ItemName"] = item.Item?.Name;
                    row["ItemId"] = item.Item?.Id;
                    row["BuyLimit"] = item.Item?.Limit ?? 0;
                    row["High"] = item.LatestPrice?.High ?? 0;
                    row["HighTimeUTC"] = UnixTimeStampToDateTime(item.LatestPrice?.HighTime);
                    row["Average"] = ((item.LatestPrice?.High + item.LatestPrice?.Low) / 2) ?? 0;
                    row["CurrentAvgTimeUTC"] = DateTime.UtcNow;
                    row["Low"] = item.LatestPrice?.Low ?? 0;
                    row["LowTimeUTC"] = UnixTimeStampToDateTime(item.LatestPrice?.LowTime);

                    dt.Rows.Add(row);
                }
            }

            return TryAppendDataTableToCsv(dt, csvFilePath);
        }

        public static async Task<bool> CreateCsvFromLatestPricesAsync(string discordUsername, string outputFilePath)
        {
            SetUserAgent(discordUsername);
            var latestPrices = await GetLatestMappedPricesAsync(discordUsername);

            DataTable dt = new DataTable();
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("ItemId", typeof(string));
            dt.Columns.Add("BuyLimit", typeof(long));
            dt.Columns.Add("High", typeof(long));
            dt.Columns.Add("HighTimeUTC", typeof(DateTime));
            dt.Columns.Add("Average", typeof(long));
            dt.Columns.Add("CurrentAvgTimeUTC", typeof(DateTime));
            dt.Columns.Add("Low", typeof(long));
            dt.Columns.Add("LowTimeUTC", typeof(DateTime));

            foreach (var item in latestPrices)
            {
                if (item != null && item.Item != null && item.Item.Name != null)
                {
                    DataRow row = dt.NewRow();

                    row["ItemName"] = item.Item?.Name;
                    row["ItemId"] = item.Item?.Id;
                    row["BuyLimit"] = item.Item?.Limit ?? 0;
                    row["High"] = item.LatestPrice?.High ?? 0;
                    row["HighTimeUTC"] = UnixTimeStampToDateTime(item.LatestPrice?.HighTime);
                    row["Average"] = ((item.LatestPrice?.High + item.LatestPrice?.Low) / 2) ?? 0;
                    row["CurrentAvgTimeUTC"] = DateTime.UtcNow;
                    row["Low"] = item.LatestPrice?.Low ?? 0;
                    row["LowTimeUTC"] = UnixTimeStampToDateTime(item.LatestPrice?.LowTime);

                    dt.Rows.Add(row);
                }
            }

            return TryConvertDataTableToCsv(dt, outputFilePath);
        }

        private static bool TryAppendDataTableToCsv(DataTable table, string filePath)
        {
            List<string> sb = new List<string>();

            // Write data rows
            foreach (DataRow row in table.Rows)
            {
                string?[] fields = row.ItemArray.Select(field => field?.ToString()).ToArray();
                sb.Add(string.Join(",", fields));
            }
            try
            {
                File.AppendAllLinesAsync(filePath, sb.AsEnumerable());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryConvertDataTableToCsv(DataTable table, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Write column headers
            string[] columnNames = table.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            // Write data rows
            foreach (DataRow row in table.Rows)
            {
                string?[] fields = row.ItemArray.Select(field => field?.ToString()).ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            try
            {
                // Save to CSV file
                File.WriteAllText(filePath, sb.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DateTime UnixTimeStampToDateTime(long? unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp ?? 0).ToUniversalTime();
            return dateTime;
        }
    }
}
