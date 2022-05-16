using DSharpPlus;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote;
public partial class QuoteCommandGroup : CommandModule
{
    [SlashCommand("list", "List quotes")]
    [SlashCommandPermissions(Permissions.SendMessages)]
    public async Task ListQuotesAsync(InteractionContext ctx)
    {

    }
}
