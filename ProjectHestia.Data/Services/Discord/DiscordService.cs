using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ProjectHestia.Data.Services.Magic;
using ProjectHestia.Data.Services.Quote;

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
    private readonly IMagicRoleService _magicRoles;

    private SlashCommandsConfiguration SlashCommandsConfiguration { get; init; }
    private InteractivityConfiguration InteractivityConfiguration { get; init; }
    
    public DiscordService(IConfiguration configuration, DiscordShardedClient client,
        IMagicRoleService magicRoles, IServiceProvider services)
    {
        _configuration = configuration;
        _client = client;
        _magicRoles = magicRoles;

        SlashCommandsConfiguration = new()
        {
            Services = services
        };
        InteractivityConfiguration = new()
        {
            ButtonBehavior = DSharpPlus.Interactivity.Enums.ButtonPaginationBehavior.DeleteButtons,
            PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.WrapAround
        };

        // We need to initalize this at least once.
        _ = services.GetRequiredService<IQuoteService>();
    }

    public async Task InitalizeAsync()
    {
        var slashExt = await _client.UseSlashCommandsAsync(SlashCommandsConfiguration);
        foreach(var slashCommand in slashExt.Values)
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
                        slashCommand.RegisterCommands(t, ulong.Parse(_configuration["Discord:HomeGuild"]));
#else
                        slashCommand.RegisterCommands(t);
#endif

            slashCommand.ContextMenuErrored += SlashCommand_ContextMenuErrored;
            slashCommand.SlashCommandErrored += SlashCommand_SlashCommandErrored;
        }

        await _client.UseInteractivityAsync(InteractivityConfiguration);

        // TOOD: Client Event Registration
        _client.ClientErrored += Client_ClientErrored;
        _client.GuildAvailable += Client_GuildAvailable;

        await _client.StartAsync();
    }

    private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs args)
    {
        _magicRoles.QueueGuildForLoading(args.Guild);

        return Task.CompletedTask;
    }

    private Task Client_ClientErrored(DiscordClient sender, ClientErrorEventArgs e)
    {
        sender.Logger.LogError(e.Exception, "Client errored");

        return Task.CompletedTask;
    }

    private Task SlashCommand_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        sender.Client.Logger.LogWarning(e.Exception, "Slash Command Errored");

        return Task.CompletedTask;
    }

    private Task SlashCommand_ContextMenuErrored(SlashCommandsExtension sender, ContextMenuErrorEventArgs e)
    {
        sender.Client.Logger.LogWarning(e.Exception, "Context Menu Errored");

        return Task.CompletedTask;
    }
}
