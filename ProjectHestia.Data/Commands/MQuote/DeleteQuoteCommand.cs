using DSharpPlus;
using DSharpPlus.Entities;
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
            var modal = new DiscordInteractionResponseBuilder()
                .WithCustomId("quote-delete")
                .WithTitle("Press Submit to Delete")
                .AddComponents(new TextInputComponent("ID", "id", "-1", quote.QuoteId.ToString()))
                .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", quote.Content, style: TextInputStyle.Paragraph, required: false))
                .AddComponents(new TextInputComponent("Image", "image", "A link to an image", quote.Image, required: false))
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
