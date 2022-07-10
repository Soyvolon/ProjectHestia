using DSharpPlus.Entities;

using ProjectHestia.Data.Structures.Data.Quotes;
using ProjectHestia.Data.Structures.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Quote;
public interface IQuoteService
{
    public Task<ActionResult<GuildQuote>> GetQuoteAsync(ulong guildId, long quoteId);
    public Task<ActionResult<GuildQuote>> UpdateQuoteAsync(Guid key, string author, string savedBy, string quote, string color, string image, long? uses = null);
    public Task<ActionResult<GuildQuote>> AddQuoteAsync(ulong guild, string author, string savedBy, string quote, string color, string image, long? uses = null, long? forceId = null);
    public Task<ActionResult<GuildQuote>> DeleteQuoteAsync(ulong guild, long quoteId);
    public Task<ActionResult<DiscordEmbedBuilder>> UseQuoteAsync(ulong guildId, long quoteId);
    public Task<ActionResult<DiscordEmbedBuilder>> UseRandomQuoteAsync(ulong guildId);
    public Task<ActionResult<List<GuildQuote>>> GetQuotesAsync(ulong guildId);
}
