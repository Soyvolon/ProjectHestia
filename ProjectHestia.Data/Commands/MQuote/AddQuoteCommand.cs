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
    [SlashCommand("add", "Add a new quote")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task AddQuoteAsync(InteractionContext ctx)
    {
        var modal = ModalBuilder.Create("quote")
            .WithTitle("Add Quote")
            .AddComponents(new TextInputComponent("Author", "author", "Author"))
            .AddComponents(new TextInputComponent("Saved By", "saved-by", "Who saved this quote..."))
            .AddComponents(new TextInputComponent("Content", "content", "What do you want to quote...", style: TextInputStyle.Paragraph, required: false))
            .AddComponents(new TextInputComponent("Color", "color", "A 6 digit Hex color (# is optional)...", "#3498db", min_length: 6, max_length: 7))
            .AddComponents(new TextInputComponent("Image", "image", "A link to an image", required: false))
            .AsEphemeral();

        await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }
}
