# OSRS.Dotnet.Tools
A library of tools for interacting with the game Oldschool Runescape's (OSRS) API's using .NET

[Install Public Package From NuGet.org](https://www.nuget.org/packages/OSRS.Dotnet.Tools)

```
using OSRS.Dotnet.Tools

#region PriceExamples
await Prices.GetLatestMappedPricesAsync("<YourDiscordName>"); // Latest prices mapped to Item object

await Prices.GetFiveMinMappedPricesAsync("<YourDiscordName>"); // Past 5min prices mapped to Item object

await Prices.GetOneHrMappedPricesAsync("<YourDiscordName>"); // Past 1hr prices mapped to Item object

await Prices.GetItemMappingsAsync("<YourDiscordName>"); // Get all item mappings

// Create a CSV from the latest mapped prices.
bool isCreateSuccess =  await Prices.CreateCsvFromLatestPricesAsync("<YourDiscordName>", @"C:\Users\foo\Desktop\LatestPrices.csv"); 

// Append to CSV with latest mapped prices.
bool isAppendSuccess =  await Prices.AppendLatestPricesToCsv("<YourDiscordName>", @"C:\Users\foo\Desktop\LatestPrices.csv");
#endregion

#region Hiscores
// Get a normal account's highscores
await Hiscores.GetHiscoreAsync(null, "Zezima"); 

// Get an Ironman's highscores
await Hiscores.GetHiscoreAsync(PlayerType.Ironman, "Iron Hyger");

#endregion
```
