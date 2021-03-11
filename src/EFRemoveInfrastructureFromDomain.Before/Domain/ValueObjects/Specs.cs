// <copyright file="Specs.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before.Domain.ValueObjects
{
	using System.Collections.Generic;

	public class Specs : ValueObject
	{
		protected Specs()
		{
		}

		public Specs(int numberOfWheels, int weight, int horsePower)
		{
			NumberOfWheels = numberOfWheels;
			Weight = weight;
			HorsePower = horsePower;
		}

		public int HorsePower { get; protected set; }

		public int NumberOfWheels { get; protected set; }

		public int Weight { get; protected set; }

		public override string ToString() => $"{NumberOfWheels} Wheels, {Weight} kg, {HorsePower} PS";

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return NumberOfWheels;
			yield return Weight;
			yield return HorsePower;
		}
	}
}