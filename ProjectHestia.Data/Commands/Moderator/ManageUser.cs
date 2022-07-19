using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

using Microsoft.Extensions.Logging;

using ProjectHestia.Data.Structures.Data.Moderator;
using ProjectHestia.Data.Structures.Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Commands.Moderator;
public partial class ModeratorCommandGroup : CommandModule
{
    [SlashCommand("manage", "Manage a user.")]
    [SlashCommandPermissions(Permissions.ManageMessages)]
    public async Task ManageUserAsync(InteractionContext ctx,
        [Option("User", "The User to manage.")]
        DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if (user is DiscordMember member)
        {
            var res = await ModeratorService.GetUserStrikesAsync(ctx.Guild.Id, member.Id);

            if (res.GetResult(out var strikes, out var err))
            {
                if (strikes.Count > 0)
                {
                    var interactivty = ctx.Client.GetInteractivity();

                    List<DiscordSelectComponentOption> options = new();
                    foreach(var strike in strikes)
                    {
                        options.Add(new DiscordSelectComponentOption(strike.LastEdit.ToString("f"), strike.Key.ToString()));
                    }

                    var builder = new DiscordWebhookBuilder()
                        .AddEmbed(EmbedTemplates.GetStandardBuilder()
                            .WithDescription("Select a strike."))
                        .AddComponents(new DiscordComponent[]
                        {
                            new DiscordSelectComponent("strikes", "Strikes", options)
                        });

                    builder.AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                    });

                    var msg = await ctx.EditResponseAsync(builder);

                    var cancel = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                    var token = cancel.Token;

                    var selectCancel = new CancellationTokenSource();
                    var selectToken = selectCancel.Token;

                    var buttonCancel = new CancellationTokenSource();
                    var buttonToken = buttonCancel.Token;

                    try
                    {
                        UserStrike? currentStrike = null;
                        var selectTask = Task.Run(async () =>
                        {
                            while (!token.IsCancellationRequested)
                            {
                                try
                                {
                                    var interaction = await interactivty.WaitForSelectAsync(msg, "strikes", selectToken);

                                    selectToken.ThrowIfCancellationRequested();

                                    if (interaction.TimedOut)
                                    {
                                        cancel.Cancel(true);
                                        selectCancel.Cancel(true);
                                        buttonCancel.Cancel(true);
                                    }
                                    else
                                    {
                                        var res = interaction.Result;
                                        await res.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                                        var val = res.Values[0];

                                        var strike = strikes.FirstOrDefault(x => x.Key.ToString() == val);

                                        if (strike is not null)
                                        {
                                            var msgBuilder = new DiscordMessageBuilder();
                                            msgBuilder.AddComponents(new DiscordComponent[]
                                            {
                                                new DiscordSelectComponent("strikes", "Strikes", options)
                                            });

                                            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator))
                                            {
                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordButtonComponent(ButtonStyle.Danger, "delete-strike", "Delete Strike"),
                                                    new DiscordButtonComponent(ButtonStyle.Danger, "confirm-delete", "Confirm Delete", true),
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                                                });
                                            }
                                            else
                                            {
                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                                                });
                                            }

                                            msgBuilder.AddEmbed(EmbedTemplates.GetStrikeBuilder(strike, member));
                                            currentStrike = strike;

                                            msg = await msg.ModifyAsync(msgBuilder);

                                            buttonCancel.Cancel();
                                        }
                                        else
                                        {
                                            cancel.Cancel(true);
                                            selectCancel.Cancel(true);
                                            buttonCancel.Cancel(true);
                                        }
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    if (!selectCancel.TryReset())
                                    {
                                        selectCancel = new CancellationTokenSource();
                                        selectToken = selectCancel.Token;
                                    }
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    ctx.Client.Logger.LogError(ex, "Interaction failed:");
                                }
                            }
                        }, token);

                        var buttonTask = Task.Run(async () =>
                        {
                            while (!token.IsCancellationRequested)
                            {
                                try
                                {
                                    var interaction = await interactivty.WaitForButtonAsync(msg, buttonToken);

                                    buttonToken.ThrowIfCancellationRequested();

                                    if (interaction.TimedOut)
                                    {
                                        cancel.Cancel(true);
                                        selectCancel.Cancel(true);
                                        buttonCancel.Cancel(true);
                                    }
                                    else
                                    {
                                        var res = interaction.Result;
                                        await res.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

                                        var msgBuilder = new DiscordMessageBuilder();
                                        switch (res.Id)
                                        {
                                            case "delete-strike":
                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordSelectComponent("strikes", "Strikes", options)
                                                });

                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "cancel-delete", "Cancel Delete"),
                                                    new DiscordButtonComponent(ButtonStyle.Danger, "confirm-delete", "Confirm Delete"),
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                                                });

                                                if (currentStrike is not null)
                                                {
                                                    msgBuilder.WithEmbed(EmbedTemplates.GetStrikeBuilder(currentStrike, member));
                                                }
                                                break;

                                            case "confirm-delete":
                                                if (currentStrike is not null)
                                                {
                                                    var delRes = await ModeratorService.DeleteUserStriekAsync(currentStrike.Key);
                                                    if (delRes.GetResult(out var delStrike, out var err))
                                                    {
                                                        var strikesRes = await ModeratorService.GetUserStrikesAsync(ctx.Guild.Id, member.Id);
                                                        if (strikesRes.GetResult(out strikes, out err))
                                                        {
                                                            if (strikes.Count > 0)
                                                            {
                                                                options.Clear();
                                                                foreach (var strike in strikes)
                                                                {
                                                                    options.Add(new DiscordSelectComponentOption(strike.LastEdit.ToString("f"), strike.Key.ToString()));
                                                                }

                                                                msgBuilder.AddEmbed(EmbedTemplates.GetSuccessBuilder()
                                                                    .WithDescription("Delted Strike. Select a new strike."));
                                                                await res.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                                                    .AddEmbed(EmbedTemplates.GetStrikeBuilder(delStrike, member))
                                                                    .WithContent("Deleted the following strike:"));
                                                            }
                                                            else
                                                            {
                                                                await res.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                                                    .AddEmbed(EmbedTemplates.GetStandardBuilder()
                                                                        .WithDescription($"There are no strikes to manage for {member.Mention}")));

                                                                cancel.Cancel(true);
                                                                selectCancel.Cancel(true);
                                                                buttonCancel.Cancel(true);
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            await res.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                                                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                                                                    .WithTitle("Error")
                                                                    .WithDescription($"Failed to manage {member.Mention}: {string.Join('\n', err)}")));

                                                            cancel.Cancel(true);
                                                            selectCancel.Cancel(true);
                                                            buttonCancel.Cancel(true);
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await res.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                                            .AddEmbed(EmbedTemplates.GetErrorBuilder()
                                                                .WithTitle("Error")
                                                                .WithDescription($"Failed to delete the strike: {string.Join('\n', err)}")));

                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    await res.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                                        .AddEmbed(EmbedTemplates.GetErrorBuilder()
                                                            .WithDescription("Can't delete a strike when no strike is selected.")));

                                                    continue;
                                                }

                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordSelectComponent("strikes", "Strikes", options)
                                                });

                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                                                });
                                                break;

                                            case "cancel-delete":
                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordSelectComponent("strikes", "Strikes", options)
                                                });

                                                msgBuilder.AddComponents(new DiscordComponent[]
                                                {
                                                    new DiscordButtonComponent(ButtonStyle.Danger, "delete-strike", "Delete Strike"),
                                                    new DiscordButtonComponent(ButtonStyle.Danger, "confirm-delete", "Confirm Delete", true),
                                                    new DiscordButtonComponent(ButtonStyle.Secondary, "close", "Close Menu")
                                                });

                                                if (currentStrike is not null)
                                                {
                                                    msgBuilder.WithEmbed(EmbedTemplates.GetStrikeBuilder(currentStrike, member));
                                                }
                                                break;

                                            case "close":
                                                cancel.Cancel(true);
                                                selectCancel.Cancel(true);
                                                buttonCancel.Cancel(true);
                                                return;
                                        }

                                        msg = await msg.ModifyAsync(msgBuilder);

                                        selectCancel.Cancel();
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    if (!buttonCancel.TryReset())
                                    {
                                        buttonCancel = new CancellationTokenSource();
                                        buttonToken = buttonCancel.Token;
                                    }
                                    continue; 
                                }
                                catch (Exception ex)
                                {
                                    ctx.Client.Logger.LogError(ex, "Interaction failed:");
                                }
                            }
                        }, token);

                        await Task.WhenAll(selectTask, buttonTask);
                    }
                    catch (TaskCanceledException) { /* Expected logic */ }

                    // Do cleanup.
                    await msg.ModifyAsync(new DiscordMessageBuilder()
                        .AddEmbed(EmbedTemplates.GetStandardBuilder()
                            .WithTitle("Manage member")
                            .WithDescription("Menu closed.")));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .AddEmbed(EmbedTemplates.GetStandardBuilder()
                            .WithDescription($"There are no strikes to manage for {member.Mention}")));
                }
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(EmbedTemplates.GetErrorBuilder()
                        .WithTitle("Error")
                        .WithDescription($"Failed to manage {member.Mention}: {string.Join('\n', err)}")));
            }
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(EmbedTemplates.GetErrorBuilder()
                    .WithTitle("Error")
                    .WithDescription($"No discord member was able to be found for {user.Username} - `{user.Id}`")));
        }
    }
}
