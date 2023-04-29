using DSharpPlus.SlashCommands;
using DSharpPlus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectHestia.Data.Services.Magic;

namespace ProjectHestia.Data.Commands.Magic;

[SlashCommandGroup("MagicRole", "Magic Role commands", true)]
[SlashCommandPermissions(Permissions.ManageRoles)]
public partial class MagicRoleCommandGroup : CommandModule
{
    private readonly IMagicRoleService _magicRoleService;

    public MagicRoleCommandGroup(IMagicRoleService magicRoleService)
    {
        _magicRoleService = magicRoleService;
    }
}
