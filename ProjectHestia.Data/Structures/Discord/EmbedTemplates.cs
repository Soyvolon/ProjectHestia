using DSharpPlus.Entities;

using ProjectHestia.Data.Structures.Data.Moderator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Structures.Discord;
public static class EmbedTemplates
{
    public static DiscordEmbedBuilder GetErrorBuilder() 
        => new DiscordEmbedBuilder()
            .WithColor(DiscordColor.DarkRed);

    public static DiscordEmbedBuilder GetStandardBuilder()
        => new DiscordEmbedBuilder()
            .WithColor(DiscordColor.CornflowerBlue);

    public static DiscordEmbedBuilder GetSuccessBuilder()
        => new DiscordEmbedBuilder()
            .WithColor(DiscordColor.SpringGreen);

    public static DiscordEmbedBuilder GetStrikeBuilder(UserStrike strike, DiscordMember member)
        => new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Orange)
            .WithTitle($"Strike for {member.Username}")
            .WithDescription($"{member.Mention} - {strike.LastEdit:f}")
            .AddField("Reason", strike.Reason);
}
