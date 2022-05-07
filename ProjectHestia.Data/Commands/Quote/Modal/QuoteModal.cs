using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote.Modal;
public class QuoteModal : ModalCommandModule
{
    [ModalCommand("quote")]
    public async Task ModifyQuoteAsync(ModalContext ctx, string author, string savedBy, string quote, string? quoteKey = null)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if(Guid.TryParse(quoteKey, out var key))
        {
            // Update quote.

        }
        else
        {
            // Add quote.

        }
    }
}
