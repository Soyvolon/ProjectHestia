using DSharpPlus.Entities;

using ProjectHestia.Data.Structures.Data.Quotes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Quote;
public interface IQuoteService
{
    public Task<GuildQuote?> GetQuoteAsync(ulong guildId, long quoteId);
}
