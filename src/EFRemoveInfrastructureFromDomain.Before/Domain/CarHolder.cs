// <copyright file="CarHolder.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before.Domain
{
	using System;
	using System.Collections.Generic;

	public class CarHolder
	{
		private readonly Action<object, string?>? lazyLoader;
		private List<Car> cars = new();

		public CarHolder(string name)
		{
			Id = Guid.NewGuid();

			Name = name;
		}

#pragma warning disable 8618
		protected CarHolder(Action<object, string?> lazyLoader)
		{
			this.lazyLoader = lazyLoader;
		}
#pragma warning restore 8618

		public virtual IReadOnlyCollection<Car> Cars => InternalCars.AsReadOnly();

		public int CarsCount => InternalCars.Count;

		public Guid Id { get; protected set; }

		public string Name { get; protected set; }

		protected List<Car> InternalCars => this.lazyLoader.Load(this, ref this.cars, nameof(CarHolder.Cars));

		public void AddCar(Car car) => InternalCars.Add(car);
	}
}