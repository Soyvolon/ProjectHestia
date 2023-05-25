using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Magic;
public partial class MagicRoleCommandGroup
{
    [SlashCommand("info", "Gets info about the magic role for this server.")]
    public async Task MagicRoleInfoAsync(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var res = await _magicRoleService.GetMagicRoleAsync(ctx.Guild);

        if (res.GetResult(out var role, out var err))
        {
            if (role is not null)
            {
                // A success occoured.
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                        .WithTitle($"Magic Role")
                        .WithDescription($"Role: <@&{role.RoleId}>\n" +
                        $"Interval: {role.Interval}\n" +
                        $"Min Members: {role.SelectionSizeMin}\n" +
                        $"Max Members: {role.SelectionSizeMax}\n" +
                        $"\n" +
                        $"Watched Channels: <#{string.Join(">, <#", role.WatchedChannels)}>\n" +
                        $"Max Messages: {role.MaxMessages}\n" +
                        $"\n" +
                        $"Use Random Removal: {role.UsePercentBootInsteadOfMaxMessages}\n" +
                        $"Starting Percentage: {role.RandomRemoveStartingPercentage}\n" +
                        $"Mod Per Message: {role.RandomRemovePercentageModPerMessage}")));
            }
            else
            {
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                        .WithTitle($"No magic role is configured.")));
            }
        }
        else
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to find the magic role.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));
        }
    }
}
