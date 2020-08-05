﻿// <auto-generated />
using System;
using InterfurCreations.AdventureGames.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20191013120331_AddedPlayerSavedData")]
    partial class AddedPlayerSavedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.AccessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HoursForRefresh");

                    b.Property<DateTime>("LastActivated");

                    b.Property<string>("PlayerId");

                    b.Property<string>("Token");

                    b.Property<string>("TokenType");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("AccessToken");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.DiscordPlayer", b =>
                {
                    b.Property<string>("PlayerId");

                    b.Property<long>("ChatId");

                    b.HasKey("PlayerId");

                    b.ToTable("DiscordPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.GameSaves", b =>
                {
                    b.Property<int>("PlayerGameSaveId");

                    b.Property<string>("PlayerId");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name");

                    b.HasKey("PlayerGameSaveId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GameSaves");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.KikPlayer", b =>
                {
                    b.Property<string>("PlayerId");

                    b.Property<string>("ChatId");

                    b.HasKey("PlayerId");

                    b.ToTable("KikPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.Player", b =>
                {
                    b.Property<string>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActiveGameSaveId");

                    b.Property<string>("Name");

                    b.Property<string>("PlayerFlag");

                    b.HasKey("PlayerId");

                    b.HasIndex("ActiveGameSaveId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionName");

                    b.Property<string>("GameName");

                    b.Property<string>("PlayerId");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerActions");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSave", b =>
                {
                    b.Property<int>("SaveId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GameName");

                    b.Property<string>("StateId");

                    b.HasKey("SaveId");

                    b.ToTable("PlayerGameSave");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSaveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("PlayerGameSaveSaveId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("PlayerGameSaveSaveId");

                    b.ToTable("PlayerGameSaveData");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerSavedData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DataName");

                    b.Property<string>("DataType");

                    b.Property<string>("DataValue");

                    b.Property<string>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerSavedData");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.TelegramPlayer", b =>
                {
                    b.Property<string>("PlayerId");

                    b.Property<long>("ChatId");

                    b.HasKey("PlayerId");

                    b.ToTable("TelegramPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.WebPlayer", b =>
                {
                    b.Property<string>("PlayerId");

                    b.Property<string>("AccessKey");

                    b.HasKey("PlayerId");

                    b.ToTable("WebPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.AccessToken", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany("AccessTokens")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.DiscordPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("DiscordPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.DiscordPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.GameSaves", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave", "PlayerGameSave")
                        .WithMany()
                        .HasForeignKey("PlayerGameSaveId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany("GameSaves")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.KikPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("KikPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.KikPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.Player", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave", "ActiveGameSave")
                        .WithMany()
                        .HasForeignKey("ActiveGameSaveId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerAction", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany("Actions")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSaveData", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave")
                        .WithMany("GameSaveData")
                        .HasForeignKey("PlayerGameSaveSaveId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerSavedData", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.TelegramPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("TelegramPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.TelegramPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.WebPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("WebPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.WebPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
