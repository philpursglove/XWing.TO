﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XWingTO.Core;

namespace XWingTO.Data
{
    public class DbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
	{
		public DbContext()
		{

		}

        public DbContext(DbContextOptions<DbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
	        optionsBuilder.UseSqlServer();
	        base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Tournament>? Tournaments { get; set; }
        public DbSet<TournamentPlayer>? TournamentPlayers { get; set; }

        public DbSet<TournamentRound>? TournamentRounds { get; set; }

        public DbSet<Game>? Games { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<Date>()
                .HaveConversion<DateConverter>()
                .HaveColumnType("date");
        }
    }

    public class DateConverter : ValueConverter<Date, DateTime>
    {
        public DateConverter() : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => Date.FromDateTime(d))
        { }
    }
}
