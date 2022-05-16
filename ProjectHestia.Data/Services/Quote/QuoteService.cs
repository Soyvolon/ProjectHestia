using DSharpPlus.Entities;

using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Services.Guild;
using ProjectHestia.Data.Structures.Data.Quotes;
using ProjectHestia.Data.Structures.Util;

namespace ProjectHestia.Data.Services.Quote;
public class QuoteService : IQuoteService
{
    private IDbContextFactory<ApplicationDbContext> DbContextFactory { get; init; }
    private IGuildService GuildService { get; init; }
    private Random Random { get; set; }

    public QuoteService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IGuildService guildService)
    {
        DbContextFactory = dbContextFactory;
        GuildService = guildService;

        Random = new();
    }

    public async Task<ActionResult<GuildQuote>> GetQuoteAsync(ulong guildId, long quoteId)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var quote = await dbContext.GuildQuotes
            .Where(x => x.QuoteId == quoteId)
            .Where(x => x.GuildId == guildId)
            .FirstOrDefaultAsync();

        if (quote is null)
            return new(false, new List<string> { "Failed to find a quote." });

        return new(true, null, quote);
    }

    public async Task<ActionResult<GuildQuote>> UpdateQuoteAsync(Guid key, string author, string savedBy, string quote, string color)
    {
        DiscordColor? colorData;
        try
        {
            colorData = new DiscordColor(color);
        }
        catch (Exception ex)
        {
            return new(false, new List<string> { $"Failed to parse a proper color from {color}.", ex.Message });
        }

        var dbContex = await DbContextFactory.CreateDbContextAsync();
        var quoteData = await dbContex.FindAsync<GuildQuote>(key);

        if (quoteData is null)
            return new(false, new List<string> { "Failed to find a quote to edit. Make sure you did not modify the edit key field." });

        quoteData.Update(author, savedBy, quote, colorData);
        await dbContex.SaveChangesAsync();

        return new(true, null, quoteData);
    }

    public async Task<ActionResult<GuildQuote>> AddQuoteAsync(ulong guild, string author, string savedBy, string quote, string color)
    {
        DiscordColor? colorData;
        try
        {
            colorData = new DiscordColor(color);
        }
        catch (Exception ex)
        {
            return new(false, new List<string> { $"Failed to parse a proper color from {color}.", ex.Message });
        }

        if (await GuildService.EnsureGuildCreated(guild))
        {
            var dbContex = await DbContextFactory.CreateDbContextAsync();

            var quoteData = new GuildQuote()
            {
                GuildId = guild,
                Author = author,
                SavedBy = savedBy,
                Content = quote,
                Color = colorData,
                LastEdit = DateTime.UtcNow
            };

            var dataHook = await dbContex.AddAsync(quoteData);
            await dbContex.SaveChangesAsync();

            await dataHook.ReloadAsync();

            return new(true, null, quoteData);
        }

        return new(false, new List<string> { "Failed to ensure a guild existed for this quote." });
    }

    public async Task<ActionResult<GuildQuote>> DeleteQuoteAsync(ulong guildId, long quoteId)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var quote = await dbContext.GuildQuotes
            .Where(x => x.QuoteId == quoteId)
            .Where(x => x.GuildId == guildId)
            .FirstOrDefaultAsync();

        if (quote is null)
            return new(false, new List<string> { "No quote found to delete." });

        dbContext.Remove(quote);
        await dbContext.SaveChangesAsync();

        return new(true, null, quote);
    }

    public async Task<ActionResult<DiscordEmbedBuilder>> UseQuoteAsync(ulong guildId, long quoteId)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var quote = await dbContext.GuildQuotes
            .Where(x => x.QuoteId == quoteId)
            .Where(x => x.GuildId == guildId)
            .FirstOrDefaultAsync();

        if (quote is null)
            return new(false, new List<string> { "Failed to find a quote to use." });

        var data = quote.UseQuote();

        await dbContext.SaveChangesAsync();

        return new(true, null, data);
    }

    public async Task<ActionResult<DiscordEmbedBuilder>> UseRandomQuoteAsync(ulong guildId)
    {
        var guild = await GuildService.GetDiscordGuildConfiguration(guildId);

        if (guild is null)
            return new(false, new List<string> { "No guild found to get quotes form." });

        var quote = guild.GuildQuotes[Random.Next(0, guild.GuildQuotes.Count)];

        return await UseQuoteAsync(guildId, quote.QuoteId);
    }
}
