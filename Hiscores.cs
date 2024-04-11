using Newtonsoft.Json;
using OSRS.Dotnet.Tools.Types;

namespace OSRS.Dotnet.Tools
{
    public class Hiscores
    {
        internal readonly static string DefaultEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool/index_lite.json?player=";
        internal readonly static string IronmanEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_ironman/index_lite.json?player=";
        internal readonly static string UIMEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_ultimate/index_lite.json?player=";
        internal readonly static string DeadmanEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_deadman/index_lite.json?player=";
        internal readonly static string SeasonalEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_seasonal/index_lite.json?player=";
        internal readonly static string TournamentEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_tournament/index_lite.json?player=";
        internal readonly static string FreshStartEndpoint = @"https://secure.runescape.com/m=hiscore_oldschool_fresh_start/index_lite.json?player=";

        private static SocketsHttpHandler _httpHandler { get; set; } = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2)
        };

        private static HttpClient _httpClient { get; set; } = new HttpClient(_httpHandler);

        internal static string GetEndpointForPlayerType(PlayerType? type)
        {
            switch (type)
            {
                case PlayerType.Ironman:
                    return IronmanEndpoint;
                case PlayerType.HardcoreIronman:
                    return UIMEndpoint;
                case PlayerType.Deadman:
                    return DeadmanEndpoint;
                case PlayerType.Seasonal:
                    return SeasonalEndpoint;
                case PlayerType.Tournament:
                    return TournamentEndpoint;
                case PlayerType.FreshStart:
                    return FreshStartEndpoint;
                case null:
                    return DefaultEndpoint;
                default:
                    return DefaultEndpoint;
            }
        }

        public static async Task<Hiscore> GetHiscoreAsync(PlayerType? type, string playerName)
        {
            HttpRequestMessage hrm = new HttpRequestMessage()
            {
                RequestUri = new Uri(GetEndpointForPlayerType(type) + playerName),
                Method = HttpMethod.Get
            };

            using (var response = await _httpClient.SendAsync(hrm))
            {
                if (response != null && response.IsSuccessStatusCode)
                {
                    var items = JsonConvert.DeserializeObject<Hiscore>(await response.Content.ReadAsStringAsync());
                    return items ?? new Hiscore();
                }
            }
            return new Hiscore();
        }
    }
}
