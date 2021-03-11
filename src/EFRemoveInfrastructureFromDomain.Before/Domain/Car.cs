// <copyright file="Car.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before.Domain
{
	using System;
	using EFRemoveInfrastructureFromDomain.Before.Domain.ValueObjects;

	public class Car
	{
		public Car(string name, Specs specs)
		{
			Id = Guid.NewGuid();

			Name = name;
			Specs = specs;
		}

#pragma warning disable 8618
		protected Car()
		{
		}
#pragma warning restore 8618

		public virtual CarHolder? CarHolder { get; protected set; }

		public Guid Id { get; protected set; }

		public string Name { get; protected set; }

		public Specs Specs { get; protected set; }

		public void TrashCar()
		{
			_ = CarHolder;

			CarHolder = null;
		}
	}
}