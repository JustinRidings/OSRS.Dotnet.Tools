namespace OSRS.Dotnet.Tools.Types
{
    //https://runescape.wiki/w/Application_programming_interface#Old_School_Hiscores
    public class Hiscore
    {
        public List<Skill?>? Skills { get; set; }
        public List<Activity?>? Activities { get; set; }
    }

    public enum PlayerType
    {
        Ironman,
        HardcoreIronman,
        UltimateIronman,
        Deadman,
        Seasonal,
        Tournament,
        FreshStart
    }
}
