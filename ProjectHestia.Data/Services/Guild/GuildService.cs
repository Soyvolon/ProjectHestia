using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Structures.Data.Guild;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Guild;
public class GuildService : IGuildService
{
    private IDbContextFactory<ApplicationDbContext> DbContextFactory { get; init; }

    public GuildService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        DbContextFactory = dbContextFactory;
    }

    public async Task<bool> EnsureGuildCreated(ulong guildId)
    {
        try
        {
            var dbContext = await DbContextFactory.CreateDbContextAsync();

            var guild = await dbContext.GuidConfigurations
                .Where(x => x.Key == guildId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (guild is null)
            {
                guild = new()
                {
                    Key = guildId,
                };

                dbContext.Add(guild);
                await dbContext.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
