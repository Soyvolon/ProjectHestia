using DSharpPlus;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote.Context;

public class AddQuoteContetMenu : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Quote", true)]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task AddQuoteAsync(ContextMenuContext context)
    {

    }
}
