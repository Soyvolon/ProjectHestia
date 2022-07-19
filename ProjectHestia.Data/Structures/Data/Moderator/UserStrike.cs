using ProjectHestia.Data.Structures.Data.Guild;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Structures.Data.Moderator;
public class UserStrike : DataObject<Guid>
{
    public GuidConfiguration Guild { get; set; }
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public string Reason { get; set; }
}
