using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRS.Dotnet.Tools.Types
{
    public class Item
    {
        public string Examine { get; set; }
        public long Id { get; set; }
        public bool IsMembers { get; set; }
        public long LowAlch { get; set; }
        public long Limit { get; set; }
        public long HighAlch { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
    }
}
