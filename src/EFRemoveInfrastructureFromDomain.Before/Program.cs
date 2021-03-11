// <copyright file="Program.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before
{
	using System;
	using System.Linq;
	using EFRemoveInfrastructureFromDomain.Before.Domain;
	using EFRemoveInfrastructureFromDomain.Before.Domain.ValueObjects;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Proxies.Internal;
	using Microsoft.Extensions.DependencyInjection;

	public class Program
	{
		public static void Main(string[] args)
		{
			var services = new ServiceCollection();
			services.AddDbContext<SampleContext>(options =>
			{
				options.UseSqlite("DataSource=local.db").UseLazyLoadingProxies();
			}, ServiceLifetime.Transient);

			var serviceProvider = services.BuildServiceProvider();

			Program.RecreateDatabase(serviceProvider.CreateScope().ServiceProvider);

			Program.AddCarHolders(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCarHolderInfo(serviceProvider.CreateScope().ServiceProvider);

			Program.GetCarHolderAndAddCar(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCarHolderInfo(serviceProvider.CreateScope().ServiceProvider);

			Program.GetCarAndSwitchCarHolder(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCarHolderInfo(serviceProvider.CreateScope().ServiceProvider);
		}

		private static void GetCarAndSwitchCarHolder(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Car car = context.Cars.Single(x => x.Name == "Tesla Model 3");
			car.TrashCar();

			Console.WriteLine($"Trashing {car.Name} ({car.Specs})");
			Console.WriteLine();

			context.SaveChanges();
		}

		private static void GetCarHolderAndAddCar(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			CarHolder carHolder = context.CarHolders.First();
			Car car = new Car("Skoda Fabia", new Specs(4, 1035, 90));
			carHolder.AddCar(car);

			Console.WriteLine($"Adding {car.Name} ({car.Specs}) to {carHolder.Name}");
			Console.WriteLine();

			context.SaveChanges();
		}

		private static void AddCarHolders(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			CarHolder carHolder1 = new CarHolder("Christopher");
			carHolder1.AddCar(new Car("Opel Meriva", new Specs(4, 1455, 120)));
			context.CarHolders.Add(carHolder1);

			CarHolder carHolder2 = new CarHolder("Christoph");
			carHolder2.AddCar(new Car("Tesla Model 3", new Specs(4, 1906, 440)));
			context.CarHolders.Add(carHolder2);

			context.SaveChanges();
		}

		private static void GetCarHolderInfo(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			foreach (CarHolder carHolder in context.CarHolders)
			{
				Console.WriteLine($"{carHolder.Name} has {carHolder.CarsCount} cars:");

				foreach (Car car in carHolder.Cars)
				{
					Console.WriteLine($"=> {car.Name} ({car.Specs})");
				}

				if (carHolder.CarsCount == 0)
				{
					Console.WriteLine(":(");
				}

				Console.WriteLine();
			}

			context.SaveChanges();
		}

		private static void RecreateDatabase(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}
	}
}