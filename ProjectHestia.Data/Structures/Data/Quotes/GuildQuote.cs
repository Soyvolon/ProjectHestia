using DSharpPlus.Entities;

using ProjectHestia.Data.Structures.Data.Guild;

namespace ProjectHestia.Data.Structures.Data.Quotes;

#nullable disable
public class GuildQuote : DataObject<Guid>
{
    public long QuoteId { get; set; }

    public string Author { get; set; }
    public string SavedBy { get; set; }
    public string Content { get; set; }
    public string Image { get; set; }

    public int? ColorRaw { get; set; }

    private DiscordColor? _color;
    public DiscordColor? Color
    {
        get
        {
            if (_color is null)
            {
                if (ColorRaw is null)
                {
                    ColorRaw = 0x3498db;
                }
                
                _color = new DiscordColor((int)ColorRaw);
            }

            return _color.Value;
        }

        set
        {
            if (value.HasValue)
            {
                ColorRaw = value.Value.Value;
                _color = value;
            }
            else
            {
                ColorRaw = 0x3498db;
                _color = new DiscordColor((int)ColorRaw);
            }
        }
    }

    public GuidConfiguration Guild { get; set; }
    public ulong GuildId { get; set; }

    public long Uses { get; set; } = 0;

    public DiscordEmbedBuilder UseQuote()
    {
        Uses++;

        return Build();
    }

    public DiscordEmbedBuilder Build()
    {
        return new DiscordEmbedBuilder()
            .WithTitle($"Quote {QuoteId} - {Author}")
            .WithDescription(Content)
            .WithFooter($"Saved By: {SavedBy} | Uses: {Uses}")
            .WithColor((DiscordColor)Color)
            .WithImageUrl(Image)
            .WithTimestamp(LastEdit);
    }

    public void Update(string author, string savedBy, string contents, DiscordColor? color, string image, long? uses, bool metadata)
    {
        if (metadata)
        {
            Author = author;
            SavedBy = savedBy;
            Color = color;
            Uses = uses ?? Uses;
        }
        else
        {
            Author = author;
            Content = contents;
            Image = image;
        }
        
        LastEdit = DateTime.UtcNow;
    }
}
#nullable enable