// <copyright file="CarHolder.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After.Domain
{
	using System;
	using System.Collections.Generic;

	public class CarHolder
	{
		private readonly List<Car> cars = new();

		public CarHolder(string name)
		{
			Id = Guid.NewGuid();

			Name = name;
		}

		public IReadOnlyCollection<Car> Cars => this.cars.AsReadOnly();

		public int CarsCount => this.cars.Count;

		public Guid Id { get; }

		public string Name { get; }

		public void AddCar(Car car) => this.cars.Add(car);
	}
}