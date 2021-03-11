// <copyright file="ValueObject.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After.Domain.ValueObjects
{
	using System.Collections.Generic;
	using System.Linq;

	public abstract class ValueObject
	{
		public static bool operator ==(ValueObject? a, ValueObject? b)
		{
			if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
			{
				return true;
			}

			if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(ValueObject? a, ValueObject? b)
		{
			return !(a == b);
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (GetType() != obj.GetType())
			{
				return false;
			}

			ValueObject valueObject = (ValueObject)obj;

			return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
		}

		public override int GetHashCode()
		{
			return GetEqualityComponents()
				.Aggregate(1, (current, obj) =>
				{
					unchecked
					{
						return (current * 23) + (obj?.GetHashCode() ?? 0);
					}
				});
		}

		protected abstract IEnumerable<object?> GetEqualityComponents();
	}
}