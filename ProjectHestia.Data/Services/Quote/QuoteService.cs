using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Structures.Data.Quotes;

namespace ProjectHestia.Data.Services.Quote;
public class QuoteService : IQuoteService
{
    private IDbContextFactory<ApplicationDbContext> DbContextFactory { get; init; }

    public QuoteService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        DbContextFactory = dbContextFactory;
    }

    public async Task<GuildQuote?> GetQuoteAsync(ulong guildId, long quoteId)
    {
        var dbContext = await DbContextFactory.CreateDbContextAsync();

        var quote = await dbContext.GuildQuotes
            .Where(x => x.QuoteId == quoteId)
            .Where(x => x.GuildId == guildId)
            .FirstOrDefaultAsync();

        return quote;
    }
}
