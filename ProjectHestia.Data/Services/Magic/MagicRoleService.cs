using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Structures.Data.Magic;
using ProjectHestia.Data.Structures.Util;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Magic;
public class MagicRoleService : IMagicRoleService
{
    private IDbContextFactory<ApplicationDbContext> DbContextFactory { get; init; }
    private ConcurrentDictionary<Guid, Timer> RoleRunners { get; init; } = new();
    private ConcurrentDictionary<ulong, DiscordGuild> GuildMaps { get; init; } = new();

    private List<DiscordGuild> ToLoad { get; init; } = new();
    private Timer LoadTimer { get; init; }

    private DiscordShardedClient DiscordClient { get; init; }

    private Random Random { get; init; } = new();

    public MagicRoleService(IDbContextFactory<ApplicationDbContext> dbContextFactory, DiscordShardedClient discordClient)
    {
        DbContextFactory = dbContextFactory;
        DiscordClient = discordClient;

        LoadTimer = new Timer(LoadGuildsAsync, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
    }

    public void QueueGuildForLoading(DiscordGuild guild)
    {
        lock (ToLoad)
        {
            ToLoad.Add(guild);
        }
    }

    private async void LoadGuildsAsync(object? state)
    {
        Dictionary<ulong, DiscordGuild> guildObjects;
        lock (ToLoad)
        {
            if (ToLoad.Count == 0)
                return;

            guildObjects = ToLoad.ToDictionary(x => x.Id);
            ToLoad.Clear();
        }

        List<ulong> keys = guildObjects.Keys.ToList();

        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var guilds = dbContext.GuidConfigurations
            .Where(x => keys.Contains(x.Key))
            .Include(x => x.MagicRole)
            .Where(x => x.MagicRole != null)
            .AsAsyncEnumerable();

        await foreach(var guild in guilds)
        {
            var guildObj = guildObjects[guild.Key];

            try
            {
                _ = RoleRunners.TryAdd(guild.MagicRole!.Key,
                        GetTimer(guild.MagicRole, guildObj));
            }
            catch
            {
                continue;
            }
        }

        // Finally, update the mappings.
        foreach(var item in  guildObjects)
        {
            GuildMaps[item.Key] = item.Value;
        }
    }

    private async Task ExecuteRoleUpdateAsync(MagicRole role, DiscordGuild guild)
    {
        try
        {
            var members = await guild.GetAllMembersAsync();
            var guildRole = guild.GetRole(role.RoleId);

            var currentMembers = members.Where(x => x.Roles.Contains(guildRole));
            var potentialMembers = members.Where(x => !currentMembers.Contains(x)).ToArray();

            foreach (var member in currentMembers)
            {
                await member.RevokeRoleAsync(guildRole);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            var memberIndices = Enumerable.Range(0, potentialMembers.Length)
                .ToList();

            int[] newMemberIndices;
            lock (Random)
            {
                int count = Random.Next(role.SelectionSizeMin, role.SelectionSizeMax);
                newMemberIndices = new int[count];
                for (int i = 0; i < count; i++)
                {
                    newMemberIndices[i] = memberIndices[Random.Next(0, memberIndices.Count)];
                }
            }

            foreach(var index in newMemberIndices)
            {
                var member = potentialMembers[index];
                await member.GrantRoleAsync(guildRole);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        catch (Exception ex)
        {
            // TODO Log error
        }
    }

    public async Task<ActionResult<MagicRole?>> GetMagicRoleAsync(DiscordGuild guild)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var guildObj = await dbContext.GuidConfigurations
            .Where(x => x.Key == guild.Id)
            .Include(x => x.MagicRole)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (guildObj is null)
            return new(false, new List<string> { "No Guild found."}, null);

        return new(true, null, guildObj.MagicRole);
    }

    public async Task<ActionResult> UpdateOrCreateMagicRole(MagicRole role)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var tracker = dbContext.Update(role);

        try
        {
            await dbContext.SaveChangesAsync();

            await tracker.ReloadAsync();

            if (GuildMaps.TryGetValue(role.GuildId, out var guild))
            {
                RoleRunners.AddOrUpdate(role.Key,
                    GetTimer(role, guild),
                    (x, y) =>
                    {
                        y.Dispose();
                        return GetTimer(role, guild);
                    });
            }

            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string> { "Failed to save the role to the database.", ex.Message });
        }
    }

    public async Task<ActionResult> DeleteMagicRole(DiscordGuild guild)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var guildObj = await dbContext.GuidConfigurations
            .Where(x => x.Key == guild.Id)
            .FirstOrDefaultAsync();

        if (guildObj is null)
            return new(true, null);

        var oldKey = guildObj.MagicRole?.Key ?? Guid.Empty;
        guildObj.MagicRole = null;

        try
        {
            await dbContext.SaveChangesAsync();

            _ = RoleRunners.TryRemove(oldKey, out _);

            return new(true, null);
        }
        catch (Exception ex)
        {
            return new(false, new List<string> { "Failed to delete the role from the database.", ex.Message });
        }
    }

    private Timer GetTimer(MagicRole role, DiscordGuild guild)
        => new(async (x) => await ExecuteRoleUpdateAsync(role, guild),
            null, TimeSpan.FromSeconds(15), role.Interval);
}
