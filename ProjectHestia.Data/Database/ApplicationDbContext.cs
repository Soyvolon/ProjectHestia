using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Structures.Data.Guild;
using ProjectHestia.Data.Structures.Data.Magic;
using ProjectHestia.Data.Structures.Data.Moderator;
using ProjectHestia.Data.Structures.Data.Quotes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Database;
#nullable disable
public class ApplicationDbContext : DbContext
{
    internal DbSet<GuildQuote> GuildQuotes { get; set; }
    internal DbSet<GuidConfiguration> GuidConfigurations { get; set; }
    internal DbSet<UserStrike> UserStrikes { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var guildQuotes = builder.Entity<GuildQuote>();
        guildQuotes.HasKey(e => e.Key);
        guildQuotes.HasOne(e => e.Guild)
            .WithMany(p => p.GuildQuotes)
            .HasForeignKey(e => e.GuildId);

        guildQuotes.HasAlternateKey(x => new { x.GuildId, x.QuoteId });
        guildQuotes.Ignore(e => e.Color);

        var guildConfigurations = builder.Entity<GuidConfiguration>();
        guildConfigurations.HasKey(e => e.Key);

        var userStrikes = builder.Entity<UserStrike>();
        userStrikes.HasKey(e => e.Key);
        userStrikes.HasOne(e => e.Guild)
            .WithMany(p => p.UserStrikes)
            .HasForeignKey(e => e.GuildId);

        var magicRole = builder.Entity<MagicRole>();
        magicRole.HasKey(e => e.Key);
        magicRole.HasOne(e => e.Guild)
            .WithOne(p => p.MagicRole)
            .HasForeignKey<MagicRole>(e => e.GuildId);
    }
}
#nullable enable