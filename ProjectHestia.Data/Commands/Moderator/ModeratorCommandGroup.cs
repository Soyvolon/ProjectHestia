using DSharpPlus;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Services.Moderator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Moderator;

[SlashCommandGroup("mod", "Moderation commands.", defaultPermission: false)]
[SlashCommandPermissions(Permissions.ManageMessages)]
public partial class ModeratorCommandGroup : CommandModule
{
    private IModeratorService ModeratorService { get; init; }

    public ModeratorCommandGroup(IModeratorService moderatorService)
    {
        ModeratorService = moderatorService;
    }
}
