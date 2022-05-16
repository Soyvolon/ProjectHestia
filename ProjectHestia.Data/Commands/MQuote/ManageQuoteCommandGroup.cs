using DSharpPlus;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Services.Quote;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.MQuote;

[SlashCommandGroup("ManageQuote", "Manage Quote commands", true)]
[SlashCommandPermissions(Permissions.ManageMessages)]
public partial class ManageQuoteCommandGroup : CommandModule
{
    private IQuoteService QuoteService { get; init; }

    public ManageQuoteCommandGroup(IQuoteService quoteService)
    {
        QuoteService = quoteService;
    }
}
