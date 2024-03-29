﻿using LA_LIGA_REKREATIVO.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace LA_LIGA_REKREATIVO.Server.Data
{
    public class LaLigaContext : DbContext
    {
        public LaLigaContext(DbContextOptions<LaLigaContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<League> Leagues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("rec");

            modelBuilder.Entity<Player>().HasKey(x => x.Id);

            modelBuilder.Entity<Team>().HasKey(x => x.Id);
            modelBuilder.Entity<Team>().HasMany(x => x.Players)
                                       .WithOne(x => x.Team);

            modelBuilder.Entity<Summary>().HasKey(x => x.Id);
            modelBuilder.Entity<Summary>().HasOne(x => x.Match)
                                          .WithMany(x => x.Summaries);

            //Match
            modelBuilder.Entity<Match>().HasKey(x => x.Id);

            modelBuilder.Entity<Match>().HasMany(x => x.Summaries)
                                       .WithOne(x => x.Match)
                                       .OnDelete(DeleteBehavior.NoAction);

            ////Configure Relationships
            //modelBuilder.Entity<Match>().HasOne(x => x.HomeTeam)
            //                            .WithMany()
            //                            .HasForeignKey("HomeTeamId");

            //modelBuilder.Entity<Match>().HasOne(x => x.AwayTeam)
            //                       .WithMany()
            //                       .HasForeignKey("AwayTeamId");


            //many to many relationship
            modelBuilder.Entity<Match>().HasMany(x => x.Players)
                                        .WithMany(x => x.Matches)
                                        .UsingEntity<MatchPlayer>(
                  x => x
                        .HasOne<Player>()
                        .WithMany()
                        .HasForeignKey(x => x.PlayerId)
                        .OnDelete(DeleteBehavior.NoAction),
                    x => x
                        .HasOne<Match>()
                        .WithMany()
                        .HasForeignKey(x => x.MatchId)
                        .OnDelete(DeleteBehavior.NoAction)
                );

            modelBuilder.Entity<League>().HasMany(x => x.Teams)
                                       .WithMany(x => x.Leagues)
                                       .UsingEntity<LeagueTeam>(
                 x => x
                       .HasOne<Team>()
                       .WithMany()
                       .HasForeignKey(x => x.TeamId)
                       .OnDelete(DeleteBehavior.NoAction),
                   x => x
                       .HasOne<League>()
                       .WithMany()
                       .HasForeignKey(x => x.LeagueId)
                       .OnDelete(DeleteBehavior.NoAction)
               );



        }
    }
}
