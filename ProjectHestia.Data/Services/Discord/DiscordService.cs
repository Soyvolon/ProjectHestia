using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.EventArgs;
using DSharpPlus.ModalCommands.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
    private ModalCommandsConfiguration ModalCommandsConfiguration { get; init; }
    
    public DiscordService(IConfiguration configuration, DiscordShardedClient client, IServiceProvider services)
    {
        _configuration = configuration;
        _client = client;

        SlashCommandsConfiguration = new()
        {
            Services = services
        };
        ModalCommandsConfiguration = new()
        {
            Services = services
        };
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
                        extension.RegisterCommands(t);
#endif

            slashCommand.ContextMenuErrored += SlashCommand_ContextMenuErrored;
            slashCommand.SlashCommandErrored += SlashCommand_SlashCommandErrored;

            // Register Modal commands
            var modalCommand = slashCommand.Client.UseModalCommands(ModalCommandsConfiguration);
            modalCommand.RegisterModals(Assembly.GetAssembly(typeof(DiscordService)) 
                ?? Assembly.GetExecutingAssembly());

            modalCommand.ModalCommandErrored += ModalCommand_ModalCommandErrored;
        }

        // TOOD: Client Event Registration
        _client.ClientErrored += Client_ClientErrored;

        await _client.StartAsync();
    }

    private Task Client_ClientErrored(DiscordClient sender, ClientErrorEventArgs e)
    {
        sender.Logger.LogError(e.Exception, "Client errored");

        return Task.CompletedTask;
    }

    private Task ModalCommand_ModalCommandErrored(ModalCommandsExtension sender, ModalCommandErrorEventArgs e)
    {
        sender.Client?.Logger.LogWarning(e.Exception, "Modal Command Errored");

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
