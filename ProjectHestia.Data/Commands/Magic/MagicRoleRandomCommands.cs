using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace ProjectHestia.Data.Commands.Magic;
public partial class MagicRoleCommandGroup : CommandModule
{
    [SlashCommand("randomremove", "Edit the magic role for this server.")]
    public async Task EditMagicRoleRandomRemoveAsync(InteractionContext ctx,
        [Option("StartingPercent", "What is the starting chance that someone gets removed" +
        "from the channel? - [0.00, 1.00).")]
        double startingPercent,

        [Option("PercentModPerMessage", "How much is the percentage modified by per message sent" +
        "by the user? - [0.00, 1.00)")]
        double perMessageMod)
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

        mRole.RandomRemoveStartingPercentage = startingPercent;
        mRole.RandomRemovePercentageModPerMessage = perMessageMod;
        mRole.UsePercentBootInsteadOfMaxMessages = true;

        if (CheckIfWatchedChannelShouldBeSet(mRole))
        {
            await SetWatchedChannels(mRole, ctx);
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

    [SlashCommand("randomremovetoggle", "Toggle the random remove function for this server.")]
    public async Task EditMagicRoleRandomRemoveAsync(InteractionContext ctx)
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

        mRole.UsePercentBootInsteadOfMaxMessages = !mRole.UsePercentBootInsteadOfMaxMessages;

        var updateRes = await _magicRoleService.UpdateOrCreateMagicRole(mRole);

        if (updateRes.GetResult(out err))
        {
            // A success occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                    .WithTitle($"Random Remove Set To: {mRole.UsePercentBootInsteadOfMaxMessages}!")
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
