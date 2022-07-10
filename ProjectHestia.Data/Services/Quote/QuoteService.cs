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

    public async Task<ActionResult<GuildQuote>> UpdateQuoteAsync(Guid key, string author, string savedBy, string quote, string color, string image, long? uses, bool metadata)
    {
        DiscordColor? colorData = null;
        if (metadata)
        {
            try
            {
                colorData = new DiscordColor(color);
            }
            catch (Exception ex)
            {
                return new(false, new List<string> { $"Failed to parse a proper color from {color}.", ex.Message });
            }
        }

        var dbContex = await DbContextFactory.CreateDbContextAsync();
        var quoteData = await dbContex.FindAsync<GuildQuote>(key);

        if (quoteData is null)
            return new(false, new List<string> { "Failed to find a quote to edit. Make sure you did not modify the edit key field." });

        if (metadata || !string.IsNullOrEmpty(quote) || !string.IsNullOrEmpty(image))
        {
            quoteData.Update(author, savedBy, quote, colorData, image, uses, metadata);
            await dbContex.SaveChangesAsync();

            return new(true, null, quoteData);
        }
        else
        {
            return new(false, new List<string> { "A quote requires either an image or content." });
        }
    }

    public async Task<ActionResult<GuildQuote>> AddQuoteAsync(ulong guild, string author, string savedBy, string quote, string color, string image, long? uses = null, long? forceId = null)
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
            await using var dbContex = await DbContextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(quote) || !string.IsNullOrEmpty(image))
            {
                var quoteData = new GuildQuote()
                {
                    GuildId = guild,
                    Author = author,
                    SavedBy = savedBy,
                    Content = quote,
                    Color = colorData,
                    Image = image,
                    Uses = uses ?? 0,
                    LastEdit = DateTime.UtcNow
                };

                if (forceId is not null)
                {
                    quoteData.QuoteId = forceId.Value;
                }
                
                if (quoteData.QuoteId == default)
                {
                    var lastQuote = await dbContex.GuildQuotes
                        .Where(x => x.GuildId == guild)
                        .OrderBy(x => x.QuoteId)
                        .LastOrDefaultAsync();
                    quoteData.QuoteId = (lastQuote?.QuoteId ?? -1) + 1;
                }

                var dataHook = await dbContex.AddAsync(quoteData);
                await dbContex.SaveChangesAsync();

                await dataHook.ReloadAsync();

                return new(true, null, quoteData);
            }
            else
            {
                return new(false, new List<string> { "A quote requires either an image or content." });
            }
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

    public async Task<ActionResult<List<GuildQuote>>> GetQuotesAsync(ulong guildId)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var quotes = await dbContext.GuildQuotes
            .Where(x => x.GuildId == guildId)
            .OrderBy(x => x.QuoteId)
            .ToListAsync();

        return new(true, null, quotes);
    }
}
