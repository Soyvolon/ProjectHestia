using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.SlashCommands;

namespace ProjectHestia.Data.Commands.MQuote;
public partial class ManageQuoteCommandGroup : CommandModule
{
    [SlashCommand("delete", "Deletes a quote")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task DeleteQuoteAsync(InteractionContext ctx,
        [Option("QuoteID", "ID of the quote to delete.")]
        long quoteId)
    {
        var quoteRes = await QuoteService.GetQuoteAsync(ctx.Guild.Id, quoteId);

        if (!quoteRes.GetResult(out var quote, out var err))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"No quote found to delete: {err[0]}")
                .AsEphemeral());
        }
        else
        {
            var colorStr = $"{quote.Color!.Value:X}";

            var modal = ModalBuilder.Create("quote-delete")
                .WithTitle("Press Submit to Delete")
                .AddComponents(new TextInputComponent("ID", "id", "-1", quote.QuoteId.ToString()))
                .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote?", quote.SavedBy))
                .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", quote.Content, style: TextInputStyle.Paragraph))
                .AddComponents(new TextInputComponent("Color", "color", "A 6 digit Hex color (# is optional)...", colorStr, min_length: 6, max_length: 7))
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
