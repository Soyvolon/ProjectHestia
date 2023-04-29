using ProjectHestia.Data.Structures.Data.Guild;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Structures.Data.Magic;
public class MagicRole : DataObject<Guid>
{
    public GuidConfiguration Guild { get; set; }
    public ulong GuildId { get; set; }

    public ulong RoleId { get; set; }
    public int SelectionSizeMin { get; set; }
    public int SelectionSizeMax { get; set; }

    public TimeSpan Interval { get; set; }
    public DateTime LastInterval { get; set; }
}
