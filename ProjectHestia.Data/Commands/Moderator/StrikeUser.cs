using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Moderator;
public partial class ModeratorCommandGroup : CommandModule
{
    [SlashCommand("strike", "Strikes a user.")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task StrikeUserAsync(InteractionContext ctx,
        [Option("User", "The User to strike.")]
        DiscordUser user,

        [Option("Reason", "Reason for the strike.")]
        string reason,

        [Option("Timeout", "The timeout time in hours for this strike.")]
        double timeout = 0,

        [Option("Alert", "Do you want to send the user the message in the resaon field?")]
        bool alert = false)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if (user is DiscordMember member)
        {

            var sRes = await ModeratorService.StrikeUserAsync(ctx.Guild.Id, member.Id, reason);

            if (sRes.GetResult(out var strike, out var err))
            {
                if (timeout > 0)
                {
                    await member.TimeoutAsync(DateTime.UtcNow.AddHours(timeout), reason.Length > 450 ? reason[..450] : reason);
                }

                if (alert)
                {
                    await member.SendMessageAsync(reason);
                }

                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetStrikeBuilder(strike, member)));

                var strikeCount = await ModeratorService.GetStrikeCountAsync(ctx.Guild.Id, member.Id);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"This is strike #{strikeCount} for {member.Mention}"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetErrorBuilder()
                        .WithTitle("Error")
                        .WithDescription($"Failed to strike {member.Mention}: {string.Join('\n', err)}")));
            }
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Error")
                    .WithDescription($"No discord member was able to be found for {user.Username} - `{user.Id}`")));
        }
    }
}
