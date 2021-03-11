// <copyright file="SampleContext.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before
{
	using EFRemoveInfrastructureFromDomain.Before.Domain;
	using Microsoft.EntityFrameworkCore;

	public class SampleContext : DbContext
	{
		public SampleContext(DbContextOptions<SampleContext> options) : base(options)
		{
		}

		public DbSet<CarHolder> CarHolders { get; set; } = default!;

		public DbSet<Car> Cars { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CarHolder>(entityTypeBuilder =>
			{
				entityTypeBuilder.Property(x => x.Id).ValueGeneratedNever();

				entityTypeBuilder.HasMany(x => x.Cars).WithOne(x => x.CarHolder);
			});

			modelBuilder.Entity<Car>(entityTypeBuilder =>
			{
				entityTypeBuilder.Property(x => x.Id).ValueGeneratedNever();

				entityTypeBuilder.OwnsOne(x => x.Specs);
			});
		}
	}
}