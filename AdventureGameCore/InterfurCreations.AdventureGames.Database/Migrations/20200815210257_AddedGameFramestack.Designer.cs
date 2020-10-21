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
    [Migration("20200815210257_AddedGameFramestack")]
    partial class AddedGameFramestack
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.AccessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HoursForRefresh")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastActivated")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("AccessToken");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.DiscordPlayer", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.HasKey("PlayerId");

                    b.ToTable("DiscordPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.GameSaves", b =>
                {
                    b.Property<int>("PlayerGameSaveId")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerGameSaveId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GameSaves");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.KikPlayer", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChatId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerId");

                    b.ToTable("KikPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.Player", b =>
                {
                    b.Property<string>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ActiveGameSaveId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerFlag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerMenuContext")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerMenuContext2")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerId");

                    b.HasIndex("ActiveGameSaveId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerActions");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerFrameStack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReturnStateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SaveId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SaveId");

                    b.ToTable("PlayerFrameStack");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSave", b =>
                {
                    b.Property<int>("SaveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GameName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SaveName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SaveId");

                    b.ToTable("PlayerGameSave");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSaveData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerGameSaveSaveId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerGameSaveSaveId");

                    b.ToTable("PlayerGameSaveData");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerSavedData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DataName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerSavedData");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.TelegramPlayer", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.HasKey("PlayerId");

                    b.ToTable("TelegramPlayers");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.WebPlayer", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccessKey")
                        .HasColumnType("nvarchar(max)");

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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.GameSaves", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave", "PlayerGameSave")
                        .WithMany()
                        .HasForeignKey("PlayerGameSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany("GameSaves")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.KikPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("KikPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.KikPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerFrameStack", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave", "Save")
                        .WithMany("FrameStack")
                        .HasForeignKey("SaveId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerGameSaveData", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.PlayerGameSave", null)
                        .WithMany("GameSaveData")
                        .HasForeignKey("PlayerGameSaveSaveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.PlayerSavedData", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithMany("PermanentData")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.TelegramPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("TelegramPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.TelegramPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterfurCreations.AdventureGames.Database.WebPlayer", b =>
                {
                    b.HasOne("InterfurCreations.AdventureGames.Database.Player", "Player")
                        .WithOne("WebPlayer")
                        .HasForeignKey("InterfurCreations.AdventureGames.Database.WebPlayer", "PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
