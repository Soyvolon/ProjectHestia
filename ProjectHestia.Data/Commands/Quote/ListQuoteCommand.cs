using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote;
public partial class QuoteCommandGroup : CommandModule
{
    [SlashCommand("list", "List quotes")]
    [SlashCommandPermissions(Permissions.SendMessages)]
    public async Task ListQuotesAsync(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder()
                .WithContent("Loading..."));

        var interact = ctx.Client.GetInteractivity();

        var quoteRes = await QuoteService.GetQuotesAsync(ctx.Guild.Id);
        if (quoteRes.GetResult(out var quotes, out var err))
        {
            if (quotes.Count <= 0)
            {
                await ctx.DeleteResponseAsync();

                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(EmbedTemplates.GetStandardBuilder()
                        .WithDescription($"No quotes to display.")));

                return;
            }

            StringBuilder builder = new();
            for (int i = 0; i < quotes.Count; i++)
            {
                var q = quotes[i];

                var part = $"{q.Author}: {q.Content}";
                part = Formatter.Strip(part);
                part = $"`{q.QuoteId}` - {part}";

                if (part.Length > 75)
                    part = part[..72] + "...";

                builder.AppendLine(part);
            }

            var full = builder.ToString();

            var pages = interact.GeneratePagesInEmbed(full, SplitType.Line, EmbedTemplates.GetStandardBuilder()
                .WithTitle($"Quotes for {ctx.Guild.Name}"));

            await ctx.DeleteResponseAsync();

            await interact.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages);
        }
        else
        {
            await ctx.DeleteResponseAsync();

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithDescription($"Failed to retrieve quote data from this server: {err.FirstOrDefault()}")));
        }
    }
}
