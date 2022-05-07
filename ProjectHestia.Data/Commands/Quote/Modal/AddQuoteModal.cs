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
public class AddQuoteModal : ModalCommandModule
{
    [ModalCommand("add-quote")]
    public async Task AddQuote(ModalContext ctx, DiscordMember author, string quote)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);


    }
}
