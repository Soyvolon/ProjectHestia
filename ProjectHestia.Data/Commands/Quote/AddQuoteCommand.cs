using DSharpPlus;
using DSharpPlus.SlashCommands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Quote;

[SlashCommandGroup("Quote", "Quote commands", true)]
[SlashCommandPermissions(Permissions.SendMessages)]
public partial class QuoteCommand : ApplicationCommandModule
{

}
