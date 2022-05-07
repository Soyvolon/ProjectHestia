using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote;
public partial class QuoteCommand : CommandModule
{
    [SlashCommand("edit", "Edit an existing quote")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task EditQuoteAsync(InteractionContext ctx,
        [Option("ID", "ID of the quote to edit")] long quoteId)
    {
        var quote = await QuoteService.GetQuoteAsync(ctx.Guild.Id, quoteId);

        if (quote is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("No quote found to edit")
                .AsEphemeral());
        }
        else
        {
            var modal = ModalBuilder.Create("quote")
                .WithTitle("Edit Quote")
                .AddComponents(new TextInputComponent("Author", "author", "Author", quote.Author))
                .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote?", quote.SavedBy))
                .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", quote.Content, style: TextInputStyle.Paragraph))
                .AsEphemeral();

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
