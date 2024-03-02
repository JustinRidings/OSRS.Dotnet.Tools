# OSRS.Dotnet.Tools
A library of tools for interacting with the game Oldschool Runescape's (OSRS) API's using .NET

Example usage:
`using OSRS.Dotnet.Tools`

`await Prices.GetLatestMappedPricesAsync("<YourDiscordName>"); // Latest prices mapped to Item object`

`await Prices.GetFiveMinMappedPricesAsync("<YourDiscordName>"); // Past 5min prices mapped to Item object`

`await Prices.GetOneHrMappedPricesAsync("<YourDiscordName>"); // Past 1hr prices mapped to Item object`

`await Prices.GetItemMappingsAsync("<YourDiscordName>"); // Get all item mappings`
