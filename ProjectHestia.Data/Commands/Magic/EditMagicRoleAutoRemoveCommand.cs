using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Magic;
public partial class MagicRoleCommandGroup
{
    [SlashCommand("autoremove", "Edit the magic role for this server.")]
    public async Task EditMagicRoleAutoRemoveAsync(InteractionContext ctx,
        [Option("MaxMessages", "How many messages can someone send before" +
        " they are removed? Set to 0 to disable this feature.")]
        long maxMessages)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var res = await _magicRoleService.GetMagicRoleAsync(ctx.Guild);
        if (!res.GetResult(out var mRole, out var err))
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to get a guild to edit.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));

            return;
        }

        mRole ??= new()
        {
            GuildId = ctx.Guild.Id
        };

        mRole.MaxMessages = maxMessages;

        if (mRole.MaxMessages != 0)
        {
            var interact = ctx.Client.GetInteractivity();

            var channelString = string.Join(", ", mRole.WatchedChannels);
            var msg = await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddComponents(new DiscordChannelSelectComponent("magic-role-autoremove-select", "Select a channel.",
                    new ChannelType[] { ChannelType.Text }, false, 1, 25))
                .WithContent("Select the role to be used as the magic role: "));

            var interactRes = await interact.WaitForSelectAsync(msg, "magic-role-autoremove-select", TimeSpan.FromMinutes(3));

            var channels = interactRes.Result.Values;

            await interactRes.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

            mRole.WatchedChannels = channels.Select(ulong.Parse).ToList();
        }

        var updateRes = await _magicRoleService.UpdateOrCreateMagicRole(mRole);

        if (updateRes.GetResult(out err))
        {
            // A success occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                    .WithTitle("Magic role updated!")
                    .WithDescription("Use the magic role info command to see the" +
                    "current configuration.")));
        }
        else
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to save the new magic role.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));
        }
    }
}
