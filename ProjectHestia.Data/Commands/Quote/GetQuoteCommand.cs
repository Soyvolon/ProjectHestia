using DSharpPlus;
using DSharpPlus.Entities;
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
    [SlashCommand("get", "Gets a quote by its ID")]
    [SlashCommandPermissions(Permissions.SendMessages)]
    public async Task GetQuoteAsync(InteractionContext ctx,
        [Option("QuoteID", "The ID of the quote to get.")]
        long quoteId)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var quoteRes = await QuoteService.UseQuoteAsync(ctx.Guild.Id, quoteId);

        if (!quoteRes.GetResult(out var quote, out var err))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder().WithDescription($"No quote found: {err[0]}")));
        }
        else
        {
            // Display the quote
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(quote));
        }
    }

    [SlashCommand("random", "Gets a random quote.")]
    [SlashCommandPermissions(Permissions.SendMessages)]
    public async Task GetRandomQuoteAsync(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var quoteRes = await QuoteService.UseRandomQuoteAsync(ctx.Guild.Id);

        if (!quoteRes.GetResult(out var quote, out var err))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder().WithDescription($"No quote found: {err[0]}")));
        }
        else
        {
            // Display the quote
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(quote));
        }
    }
}
