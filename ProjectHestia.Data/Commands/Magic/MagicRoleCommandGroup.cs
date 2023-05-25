using DSharpPlus.SlashCommands;
using DSharpPlus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHestia.Data.Services.Magic;
using ProjectHestia.Data.Structures.Data.Magic;
using System.Data;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace ProjectHestia.Data.Commands.Magic;

[SlashCommandGroup("MagicRole", "Magic Role commands", true)]
[SlashCommandPermissions(Permissions.ManageRoles)]
public partial class MagicRoleCommandGroup : CommandModule
{
    private readonly IMagicRoleService _magicRoleService;

    public MagicRoleCommandGroup(IMagicRoleService magicRoleService)
    {
        _magicRoleService = magicRoleService;
    }

    public static bool CheckIfWatchedChannelShouldBeSet(MagicRole mRole)
    {
        return (mRole.RandomRemoveStartingPercentage != 0
                && mRole.RandomRemovePercentageModPerMessage != 0)
            || (!mRole.UsePercentBootInsteadOfMaxMessages 
                && mRole.MaxMessages != 0);
    }

    public static async Task SetWatchedChannels(MagicRole mRole, InteractionContext ctx)
    {
        var interact = ctx.Client.GetInteractivity();

        var channelString = string.Join(", ", mRole.WatchedChannels);
        var msg = await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
            .AddComponents(new DiscordChannelSelectComponent("magic-role-autoremove-select", "Select a channel.",
                new ChannelType[] { ChannelType.Text }, false, 1, 25))
            .WithContent("Select the channel to be watched for auto removal: "));

        var interactRes = await interact.WaitForSelectAsync(msg, "magic-role-autoremove-select", TimeSpan.FromMinutes(3));

        var channels = interactRes.Result.Values;

        await interactRes.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

        mRole.WatchedChannels = channels.Select(ulong.Parse).ToList();
    }
}
