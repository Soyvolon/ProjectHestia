// See https://aka.ms/new-console-template for more information
using CloudNine.Core.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Services.Guild;
using ProjectHestia.Data.Services.Quote;
using ProjectHestia.Data.Structures.Data.Quotes;

Console.WriteLine("Hello, World!");
Console.WriteLine("Opening databases ...");

ServiceCollection services = new();

using FileStream fs = new FileStream(Path.Join("Config", "database_config.json"), FileMode.Open, FileAccess.Read, FileShare.Read);
using StreamReader sr = new StreamReader(fs);
var json = sr.ReadToEnd();

var config = JsonConvert.DeserializeObject<DatabaseConfiguration>(json);

services.AddDbContext<CloudNineDatabaseModel>(ServiceLifetime.Transient, ServiceLifetime.Scoped)
    .AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseNpgsql(config.NpgSql))
    .AddSingleton<IQuoteService, QuoteService>()
    .AddSingleton<IGuildService, GuildService>();

var provider = services.BuildServiceProvider();

using var oldDb = provider.GetRequiredService<CloudNineDatabaseModel>();
using var newDb = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();
var quoteService = provider.GetRequiredService<IQuoteService>();

Console.WriteLine("Reading old data ...");

List<GuildQuote> quotes = new();
var oldData = await oldDb.ServerConfigurations
    .ToListAsync();

Console.WriteLine("Got old data, strating write...");

foreach (var x in oldData)
{
    var order = x.Quotes.Values.OrderBy(x => x.Id);
    foreach (var q in order)
    {
        string color = q.ColorValue is null ? "3498db" : q.ColorValue.Value.ToString("X");
        var res = await quoteService.AddQuoteAsync(x.Id, q.Author, q.SavedBy, q.Content, color, q.Attachment, q.Uses);
        if (!res.GetResult(out var err))
        {
            Console.WriteLine($"{x.Id}:{q.Id}-{err[0]}");
            // Colors cause a lot of problems, so lets try this again
            Console.WriteLine($"Retrying for {x.Id}:{q.Id}\n");
            res = await quoteService.AddQuoteAsync(x.Id, q.Author, q.SavedBy, q.Content, "3498db", q.Attachment, q.Uses);
            if(!res.GetResult(out err))
            {
                Console.WriteLine($"{x.Id}:{q.Id}-{err[0]}\n");
            }
        }
    }
}

Console.WriteLine("Finished moving quote data.");
