using DSharpPlus.Entities;

using ProjectHestia.Data.Structures.Data.Magic;
using ProjectHestia.Data.Structures.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Magic;
public interface IMagicRoleService
{
    public void QueueGuildForLoading(DiscordGuild guild);
    public Task<ActionResult<MagicRole?>> GetMagicRoleAsync(DiscordGuild guild);
    public Task<ActionResult> UpdateOrCreateMagicRole(MagicRole role);
    public Task<ActionResult> DeleteMagicRole(DiscordGuild guild);
}
