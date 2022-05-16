using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.MQuote.Context;

public class AddQuoteContetMenu : CommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Quote", true)]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task AddQuoteAsync(ContextMenuContext ctx)
    {
        var modal = ModalBuilder.Create("quote")
            .WithTitle("Add Quote")
            .AddComponents(new TextInputComponent("Author", "author", "Author", ctx.TargetMessage.Author.Username))
            .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote?", ctx.User.Username))
            .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", ctx.TargetMessage.Content, style: TextInputStyle.Paragraph))
            .AddComponents(new TextInputComponent("Color", "color", "A 6 digit Hex color (# is optional)...", "#3498db", min_length: 6, max_length: 7))
            .AsEphemeral();

        await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }
}
