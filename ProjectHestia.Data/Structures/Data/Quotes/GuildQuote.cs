using ProjectHestia.Data.Structures.Data.Guild;

namespace ProjectHestia.Data.Structures.Data.Quotes;

#nullable disable
public class GuildQuote : DataObject<Guid>
{
    public long QuoteId { get; set; }

    public string Author { get; set; }
    public string SavedBy { get; set; }
    public string Content { get; set; }

    public GuidConfiguration Guild { get; set; }
    public ulong GuildId { get; set; }
}
#nullable enable