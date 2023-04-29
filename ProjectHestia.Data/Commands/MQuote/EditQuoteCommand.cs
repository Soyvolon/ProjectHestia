using DSharpPlus;
using DSharpPlus.Entities;
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
        [Option("QuoteID", "ID of the quote to edit")] 
        long quoteId,

        [Option("EditType", "What data are you editing?"), Choice("Metadata", "metadata"), 
            Choice("Content", "content")]
        string editType)
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

            var modal = editType switch
            {
                "content" => new DiscordInteractionResponseBuilder()
                    .WithCustomId("quote-edit-content")
                    .WithTitle("Edit Quote")
                    .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                    .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", quote.Content, style: TextInputStyle.Paragraph, required: false))
                    .AddComponents(new TextInputComponent("Image", "image", "A link to an image!", required: false))
                    .AddComponents(new TextInputComponent("Edit Key (Do Not Change)", "key", value: key, min_length: key.Length, max_length: key.Length))
                    .AsEphemeral(),

                "metadata" => new DiscordInteractionResponseBuilder()
                    .WithCustomId("quote-edit-metadata")
                    .WithTitle("Edit Quote Metadata")
                    .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                    .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote?", quote.SavedBy))
                    .AddComponents(new TextInputComponent("Color", "color", "A 6 digit Hex color (# is optional)...", colorStr, min_length: 6, max_length: 7))
                    .AddComponents(new TextInputComponent("Uses", "uses", "How many uses has this quote had?", quote.Uses.ToString()))
                    .AddComponents(new TextInputComponent("Edit Key (Do Not Change)", "key", value: key, min_length: key.Length, max_length: key.Length))
                    .AsEphemeral(),

                _ => null
            };

            if (modal is not null)
                await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
