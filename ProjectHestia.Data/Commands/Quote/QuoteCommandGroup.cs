using DSharpPlus;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Services.Quote;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote;

[SlashCommandGroup("Quote", "Quote commands", true)]
[SlashCommandPermissions(Permissions.SendMessages)]
public partial class QuoteCommand : CommandModule
{
    private IQuoteService QuoteService { get; init; }

    public QuoteCommand(IQuoteService quoteService)
    {
        QuoteService = quoteService;
    }
}
