namespace OSRS.Dotnet.Tools.Types
{
    public class Item
    {
        public string Examine { get; set; } = default!;
        public long Id { get; set; }
        public bool IsMembers { get; set; }
        public long LowAlch { get; set; }
        public long Limit { get; set; }
        public long HighAlch { get; set; }
        public string Icon { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
