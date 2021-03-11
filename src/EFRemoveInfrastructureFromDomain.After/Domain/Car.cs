// <copyright file="Car.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After.Domain
{
	using System;
	using EFRemoveInfrastructureFromDomain.After.Domain.ValueObjects;

	public class Car
	{
		public Car(string name, Specs specs)
		{
			Id = Guid.NewGuid();

			Name = name;
			Specs = specs;
		}

		public Guid Id { get; }

		public string Name { get; }

		public Specs Specs { get; }

		public CarHolder? CarHolder { get; protected set; }

		public void TrashCar() => CarHolder = null;
	}
}