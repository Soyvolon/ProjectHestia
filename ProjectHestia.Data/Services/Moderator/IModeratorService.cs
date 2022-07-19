using ProjectHestia.Data.Structures.Data.Moderator;
using ProjectHestia.Data.Structures.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Moderator;
public interface IModeratorService
{
    public Task<ActionResult<UserStrike>> StrikeUserAsync(ulong guild, ulong user, string reason);
    public Task<ActionResult<List<UserStrike>>> GetUserStrikesAsync(ulong guild, ulong user);
    public Task<ActionResult<UserStrike>> DeleteUserStriekAsync(Guid strikeId);
    public Task<int> GetStrikeCountAsync(ulong guild, ulong user);
}
