using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Structures.Data.Moderator;
using ProjectHestia.Data.Structures.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Moderator;
public class ModeratorService : IModeratorService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ModeratorService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<ActionResult<UserStrike>> DeleteUserStriekAsync(Guid strikeId)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var strike = await _dbContext.UserStrikes
            .Where(x => x.Key == strikeId)
            .FirstOrDefaultAsync();

        if (strike is not null)
        {
            _dbContext.Remove(strike);
            await _dbContext.SaveChangesAsync();

            return new(true, null, strike);
        }

        return new(false, new List<string> { "No strike was found to delete." });
    }

    public async Task<ActionResult<List<UserStrike>>> GetUserStrikesAsync(ulong guild, ulong user)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var strikes = await _dbContext.UserStrikes
            .Where(x => x.GuildId == guild)
            .Where(x => x.UserId == user)
            .ToListAsync();

        return new(true, null, strikes);
    }

    public async Task<ActionResult<UserStrike>> StrikeUserAsync(ulong guild, ulong user, string reason)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        UserStrike strike = new()
        {
            GuildId = guild,
            UserId = user,
            Reason = reason,
            LastEdit = DateTime.UtcNow
        };

        await _dbContext.AddAsync(strike);
        await _dbContext.SaveChangesAsync();

        return new(true, null, strike);
    }

    public async Task<int> GetStrikeCountAsync(ulong guild, ulong user)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var strikes = await _dbContext.UserStrikes
            .Where(x => x.GuildId == guild)
            .Where(x => x.UserId == user)
            .CountAsync();

        return strikes;
    }
}
