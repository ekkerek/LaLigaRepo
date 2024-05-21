﻿// <auto-generated />
using System;
using LA_LIGA_REKREATIVO.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    [DbContext(typeof(LaLigaContext))]
    partial class LaLigaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("rec")
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.League", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Coefficient")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Logo")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Leagues", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.LeagueTeam", b =>
                {
                    b.Property<int>("LeagueId")
                        .HasColumnType("integer");

                    b.Property<int>("TeamId")
                        .HasColumnType("integer");

                    b.HasKey("LeagueId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("LeagueTeam", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AwayTeamGoals")
                        .HasColumnType("integer");

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("integer");

                    b.Property<string>("GamePlace")
                        .HasColumnType("text");

                    b.Property<int>("GameRound")
                        .HasColumnType("integer");

                    b.Property<DateTime>("GameTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("HomeTeamGoals")
                        .HasColumnType("integer");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("LeagueId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LeagueId");

                    b.ToTable("Matches", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.MatchPlayer", b =>
                {
                    b.Property<int>("MatchId")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.HasKey("MatchId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("MatchPlayer", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsGk")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Picture")
                        .HasColumnType("text");

                    b.Property<int>("TeamId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Players", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Summary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("MatchId")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("Time")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Summaries", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LogoSrc")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ParticipantOf")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teams", "rec");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.LeagueTeam", b =>
                {
                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.League", null)
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Match", b =>
                {
                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.League", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.MatchPlayer", b =>
                {
                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Match", null)
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Player", null)
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Player", b =>
                {
                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Summary", b =>
                {
                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Match", "Match")
                        .WithMany("Summaries")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LA_LIGA_REKREATIVO.Server.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Match", b =>
                {
                    b.Navigation("Summaries");
                });

            modelBuilder.Entity("LA_LIGA_REKREATIVO.Server.Models.Team", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
