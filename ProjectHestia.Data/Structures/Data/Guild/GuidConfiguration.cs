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
}
