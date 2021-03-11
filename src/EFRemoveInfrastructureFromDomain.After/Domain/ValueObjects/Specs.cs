// <copyright file="Specs.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After.Domain.ValueObjects
{
	using System.Collections.Generic;

	public class Specs : ValueObject
	{
		public Specs(int numberOfWheels, int weight, int horsePower)
		{
			NumberOfWheels = numberOfWheels;
			Weight = weight;
			HorsePower = horsePower;
		}

		public int HorsePower { get; }

		public int NumberOfWheels { get; }

		public int Weight { get; }

		public override string ToString() => $"{NumberOfWheels} Wheels, {Weight} kg, {HorsePower} PS";

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return NumberOfWheels;
			yield return Weight;
			yield return HorsePower;
		}
	}
}