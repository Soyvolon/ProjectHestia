using DSharpPlus.Entities;

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
}
