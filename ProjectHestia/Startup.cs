using DSharpPlus;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;
using ProjectHestia.Data.Services.Discord;
using ProjectHestia.Data.Services.Guild;
using ProjectHestia.Data.Services.Magic;
using ProjectHestia.Data.Services.Moderator;
using ProjectHestia.Data.Services.Quote;

namespace ProjectHestia;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(
                Configuration.GetConnectionString("database"))
            #if DEBUG
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            #endif
            , ServiceLifetime.Singleton);

        services
            .AddScoped(p =>
                p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        services
            .AddSingleton<IDiscordService, DiscordService>()
            .AddSingleton<IQuoteService, QuoteService>()
            .AddSingleton<IGuildService, GuildService>()
            .AddSingleton<IModeratorService, ModeratorService>()
            .AddSingleton<IMagicRoleService, MagicRoleService>()
            .AddSingleton(new DiscordConfiguration()
            {
                Token = Configuration["Discord:Token"]
            })
            .AddSingleton<DiscordShardedClient>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using var scope = app.ApplicationServices.CreateScope();
        var dbFac = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var db = dbFac.CreateDbContext();
        ApplyDatabaseMigrations(db);

        var discordClient = scope.ServiceProvider.GetRequiredService<IDiscordService>();
        _ = Task.Run(async () => {
            try
            {
                await discordClient.InitalizeAsync();
            }
            catch (Exception ex)
            {

            }
        });

        app.UseForwardedHeaders(new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }

    private static void ApplyDatabaseMigrations(DbContext database)
    {
        if (!(database.Database.GetPendingMigrations()).Any())
        {
            return;
        }

        database.Database.Migrate();
        database.SaveChanges();
    }
}
