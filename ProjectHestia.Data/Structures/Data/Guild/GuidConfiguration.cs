using ProjectHestia.Data.Structures.Data.Magic;
using ProjectHestia.Data.Structures.Data.Moderator;
using ProjectHestia.Data.Structures.Data.Quotes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Structures.Data.Guild;
public class GuidConfiguration : DataObject<ulong>
{
    public List<GuildQuote> GuildQuotes { get; set; } = new();
    public List<UserStrike> UserStrikes { get; set; } = new();
    public MagicRole? MagicRole { get; set; }
}
