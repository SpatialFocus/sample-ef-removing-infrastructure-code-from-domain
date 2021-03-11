// <copyright file="LazyLoaderExtensions.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.Before.Domain
{
	using System;
	using System.Runtime.CompilerServices;

	public static class LazyLoaderExtensions
	{
		public static TRelated Load<TRelated>(this Action<object, string?>? lazyLoader, object entity, ref TRelated navigationField,
			[CallerMemberName] string? navigationName = null) where TRelated : class
		{
			lazyLoader?.Invoke(entity, navigationName);

			return navigationField;
		}
	}
}