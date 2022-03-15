using DSharpPlus;
using DSharpPlus.SlashCommands;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Discord;
public class DiscordService : IDiscordService
{
    private readonly IConfiguration _configuration;
    private readonly DiscordShardedClient _client;

    private SlashCommandsConfiguration SlashCommandsConfiguration { get; init; }
    
    public DiscordService(IConfiguration configuration, DiscordShardedClient client)
    {

    }

    public async Task InitalizeAsync()
    {
        var slashExt = await _client.UseSlashCommandsAsync(SlashCommandsConfiguration);
        foreach(var extension in slashExt.Values)
        {
            // Command Registration
            List<Type> parentGroups = new() { /* typeof(ParentClass) */ };
            List<Type> ignoreGrouping = new();
            foreach (var p in parentGroups)
                ignoreGrouping.AddRange(p.GetNestedTypes());

            var types = Assembly.GetAssembly(typeof(DiscordService))?.GetTypes();
            if (types is not null)
                foreach (var t in types)
                    if (t.IsSubclassOf(typeof(ApplicationCommandModule))
                        && !parentGroups.Any(x => t.IsSubclassOf(x))
                        && !ignoreGrouping.Contains(t))
#if DEBUG
                        extension.RegisterCommands(t, ulong.Parse(_configuration["Discord:HomeGuild"]));
#else
                        extension.RegisterCommands(t);
#endif

            // TODO: Event Registration
        }

        // TOOD: Client Event Registration

        await _client.StartAsync();
    }
}
