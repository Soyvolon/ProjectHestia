using DSharpPlus;
using DSharpPlus.Entities;
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
    [SlashCommand("delete", "Delete the magic role for this server.")]
    public async Task DeleteMagicRoleAsync(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var res = await _magicRoleService.DeleteMagicRole(ctx.Guild);

        if (res.GetResult(out var err))
        {
            // A success occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                    .WithTitle("Deleted the magic role.")
                    .WithDescription("The magic role is now deleted. Want to add it again? Just use " +
                    "the edit command again!")));
        }
        else
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to delete the magic role.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));
        }
    }
}
