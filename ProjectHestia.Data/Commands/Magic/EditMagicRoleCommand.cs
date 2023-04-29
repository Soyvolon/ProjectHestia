using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Magic;
public partial class MagicRoleCommandGroup
{
    [SlashCommand("edit", "Edit the magic role for this server.")]
    public async Task EditMagicRoleAsync(InteractionContext ctx,
        [Option("Interval", "How long between each change of members with the magic role" +
        " (in minutes).")]
        long interval,

        [Option("MinimumMembers", "The Minimum number of members that will be selected per magic role" +
        " change.")]
        long minMembers = 1,

        [Option("MaxMembers", "The Maximum number of members that will be selected per magic role" +
        " change.")]
        long maxMembers = -1)
    {
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var res = await _magicRoleService.GetMagicRoleAsync(ctx.Guild);
        if (!res.GetResult(out var mRole, out var err))
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to get a guild to edit.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));

            return;
        }

        mRole ??= new()
        {
            GuildId = ctx.Guild.Id
        };

        mRole.SelectionSizeMin = (int)(minMembers > 1 ? minMembers : 1);

        // Offset random value generators.
        maxMembers += 1;
        mRole.SelectionSizeMax = (int)(maxMembers > minMembers ? maxMembers : minMembers + 1);
        mRole.Interval = TimeSpan.FromMinutes(interval);

        var interact = ctx.Client.GetInteractivity();

        var msg = await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
            .AddComponents(new DiscordRoleSelectComponent("magic-role-select", "", false, 1, 1))
            .WithContent("Select the role to be used as the magic role: "));

        var interactRes = await interact.WaitForSelectAsync(msg, "magic-role-select", TimeSpan.FromMinutes(1));

        var role = interactRes.Result.Values.FirstOrDefault();
        if (role is null)
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("No role was selected by the user.")));

            return;
        }

        mRole.RoleId = ulong.Parse(role);

        var updateRes = await _magicRoleService.UpdateOrCreateMagicRole(mRole);

        if (updateRes.GetResult(out err))
        {
            // A success occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetSuccessBuilder()
                    .WithTitle("Magic role updated!")
                    .WithDescription("Use the magic role info command to see the" +
                    "current configuration.")));
        }
        else
        {
            // An error occoured.
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Failed to save the new magic role.")
                    .WithDescription(err?.FirstOrDefault() ?? "")));
        }
    }
}
