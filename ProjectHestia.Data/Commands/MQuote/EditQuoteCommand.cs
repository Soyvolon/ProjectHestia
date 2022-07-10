using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.MQuote;
public partial class ManageQuoteCommandGroup : CommandModule
{
    [SlashCommand("edit", "Edit an existing quote")]
    public async Task EditQuoteAsync(InteractionContext ctx,
        [Option("QuoteID", "ID of the quote to edit")] long quoteId)
    {
        var quoteRes = await QuoteService.GetQuoteAsync(ctx.Guild.Id, quoteId);

        if (!quoteRes.GetResult(out var quote, out var err))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"No quote found to edit: {err[0]}")
                .AsEphemeral());
        }
        else
        {
            var key = quote.Key.ToString();
            var colorStr = $"{quote.Color!.Value:X}";

            var modal = ModalBuilder.Create("quote-edit")
                .WithTitle("Edit Quote")
                .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote?", quote.SavedBy))
                .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", quote.Content, style: TextInputStyle.Paragraph, required: false))
                .AddComponents(new TextInputComponent("Color", "color", "A 6 digit Hex color (# is optional)...", colorStr, min_length: 6, max_length: 7))
                .AddComponents(new TextInputComponent("Image", "image", "A link to an image!", required: false))
                .AddComponents(new TextInputComponent("Edit Key (Do Not Change)", "key", value: key, min_length: key.Length, max_length: key.Length))
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
