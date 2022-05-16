using DSharpPlus;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Services.Quote;

namespace ProjectHestia.Data.Commands.Quote;

[SlashCommandGroup("quote", "Quote commands")]
[SlashCommandPermissions(Permissions.SendMessages)]
public partial class QuoteCommandGroup : CommandModule
{
    private IQuoteService QuoteService { get; init; }

    public QuoteCommandGroup(IQuoteService quoteService)
    {
        QuoteService = quoteService;
    }


}
