using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;

using ProjectHestia.Data.Services.Quote;
using ProjectHestia.Data.Structures.Data.Quotes;
using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.MQuote.Modal;
public class QuoteModal : ModalCommandModule
{
    private IQuoteService QuoteService { get; init; }

    public QuoteModal(IQuoteService quoteService)
    {
        QuoteService = quoteService;
    }

    [ModalCommand("quote-edit-metadata")]
    public async Task ModifyQuoteMetadataAsync(ModalContext ctx, string author, string savedBy, string color, string uses, string? quoteKey = null)
        => await ModifyQuoteAsync(ctx, author, savedBy, null!, color, null!, uses, quoteKey, true);

    [ModalCommand("quote-edit-content")]
    public async Task ModifyQuoteContentAsync(ModalContext ctx, string author, string quote, string image, string? quoteKey = null)
        => await ModifyQuoteAsync(ctx, author, null!, quote, null!, image, null!, quoteKey, false);

    public async Task ModifyQuoteAsync(ModalContext ctx, string author, string savedBy, string quote, string color, string image, string uses, string? quoteKey = null, bool metadata = false)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        GuildQuote? quoteData;
        List<string>? err;

        _ = long.TryParse(uses, out var usesInt);

        if (Guid.TryParse(quoteKey, out var key))
        {
            // Update quote.
            var res = await QuoteService.UpdateQuoteAsync(key, author, savedBy, quote, color, image, usesInt, metadata);
            _ = res.GetResult(out quoteData, out err);
        }
        else
        {
            // Add quote.
            var res = await QuoteService.AddQuoteAsync(ctx.Guild.Id, author, savedBy, quote, color, image);
            _ = res.GetResult(out quoteData, out err);
        }

        if (quoteData is null)
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to edit/add a quote.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));
        }
        else
        {
            // Display the quote
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(quoteData.Build()));
        }
    }

    [ModalCommand("quote")]
    public async Task ModifyQuoteAsync(ModalContext ctx, string author, string savedBy, string quote, string color, string image)
        => await ModifyQuoteAsync(ctx, author, savedBy, quote, color, image, null);

    [ModalCommand("quote-delete")]
    public async Task DeleteQuoteAsync(ModalContext ctx, string id, string author, string quote, string image)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if(long.TryParse(id, out var quoteId))
        {
            var res = await QuoteService.DeleteQuoteAsync(ctx.Guild.Id, quoteId);

            if(res.GetResult(out var resData, out var err))
            {
                // Display the quote
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(resData.Build()
                        .WithTitle($"Deleted Quote {quoteId}")));
            }
            else
            {
                // An error occoured.
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetErrorBuilder()
                        .WithTitle("Failed to edit/add a quote.")
                        .WithDescription(err?.FirstOrDefault() ?? "")));
            }
        }
        else
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to delete a quote.")
                    .WithDescription($"Could not parse {id} into a valid quote ID. Make sure not to change this value.")));
        }
    }
}
