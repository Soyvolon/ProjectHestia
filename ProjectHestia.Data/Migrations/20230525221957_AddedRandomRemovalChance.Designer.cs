﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ProjectHestia.Data.Database;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230525221957_AddedRandomRemovalChance")]
    partial class AddedRandomRemovalChance
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Guild.GuidConfiguration", b =>
                {
                    b.Property<decimal>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastEdit")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Key");

                    b.ToTable("GuidConfigurations");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Magic.MagicRole", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<TimeSpan>("Interval")
                        .HasColumnType("interval");

                    b.Property<DateTime>("LastEdit")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastInterval")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("MaxMessages")
                        .HasColumnType("bigint");

                    b.Property<double>("RandomRemovePercentageModPerMessage")
                        .HasColumnType("double precision");

                    b.Property<double>("RandomRemoveStartingPercentage")
                        .HasColumnType("double precision");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("SelectionSizeMax")
                        .HasColumnType("integer");

                    b.Property<int>("SelectionSizeMin")
                        .HasColumnType("integer");

                    b.Property<bool>("UsePercentBootInsteadOfMaxMessages")
                        .HasColumnType("boolean");

                    b.Property<decimal[]>("WatchedChannels")
                        .IsRequired()
                        .HasColumnType("numeric(20,0)[]");

                    b.HasKey("Key");

                    b.HasIndex("GuildId")
                        .IsUnique();

                    b.ToTable("MagicRole");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Moderator.UserStrike", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastEdit")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Key");

                    b.HasIndex("GuildId");

                    b.ToTable("UserStrikes");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Quotes.GuildQuote", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<int?>("ColorRaw")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastEdit")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("QuoteId")
                        .HasColumnType("bigint");

                    b.Property<string>("SavedBy")
                        .HasColumnType("text");

                    b.Property<long>("Uses")
                        .HasColumnType("bigint");

                    b.HasKey("Key");

                    b.HasAlternateKey("GuildId", "QuoteId");

                    b.ToTable("GuildQuotes");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Magic.MagicRole", b =>
                {
                    b.HasOne("ProjectHestia.Data.Structures.Data.Guild.GuidConfiguration", "Guild")
                        .WithOne("MagicRole")
                        .HasForeignKey("ProjectHestia.Data.Structures.Data.Magic.MagicRole", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Moderator.UserStrike", b =>
                {
                    b.HasOne("ProjectHestia.Data.Structures.Data.Guild.GuidConfiguration", "Guild")
                        .WithMany("UserStrikes")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Quotes.GuildQuote", b =>
                {
                    b.HasOne("ProjectHestia.Data.Structures.Data.Guild.GuidConfiguration", "Guild")
                        .WithMany("GuildQuotes")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ProjectHestia.Data.Structures.Data.Guild.GuidConfiguration", b =>
                {
                    b.Navigation("GuildQuotes");

                    b.Navigation("MagicRole");

                    b.Navigation("UserStrikes");
                });
#pragma warning restore 612, 618
        }
    }
}
