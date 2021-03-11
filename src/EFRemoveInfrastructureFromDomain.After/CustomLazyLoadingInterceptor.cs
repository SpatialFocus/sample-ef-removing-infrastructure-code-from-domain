// <copyright file="CustomLazyLoadingInterceptor.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Reflection;
	using Castle.DynamicProxy;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Proxies.Internal;

	// Workaround for https://github.com/dotnet/efcore/issues/24064
#pragma warning disable EF1001 // Internal EF Core API usage.
	public class CustomLazyLoadingInterceptor : IInterceptor
	{
		private static readonly PropertyInfo
			lazyLoaderProperty = typeof(IProxyLazyLoader).GetProperty(nameof(IProxyLazyLoader.LazyLoader))!;

		private static readonly MethodInfo lazyLoaderGetter = CustomLazyLoadingInterceptor.lazyLoaderProperty!.GetMethod!;

		private static readonly MethodInfo lazyLoaderSetter = CustomLazyLoadingInterceptor.lazyLoaderProperty!.SetMethod!;

		private readonly IEntityType entityType;
		private ILazyLoader loader;

		public CustomLazyLoadingInterceptor([NotNull] IEntityType entityType, [NotNull] ILazyLoader loader)
		{
			this.entityType = entityType;
			this.loader = loader;
		}

		public virtual void Intercept(IInvocation invocation)
		{
			var methodName = invocation.Method.Name;

			if (CustomLazyLoadingInterceptor.lazyLoaderGetter.Equals(invocation.Method))
			{
				invocation.ReturnValue = this.loader;
			}
			else if (CustomLazyLoadingInterceptor.lazyLoaderSetter.Equals(invocation.Method))
			{
				this.loader = (ILazyLoader)invocation.Arguments[0];
			}
			else
			{
				if (methodName.StartsWith("get_", StringComparison.Ordinal))
				{
					var navigationName = methodName.Substring(4);
					var navigationBase = this.entityType.FindNavigation(navigationName) ??
						(INavigationBase)this.entityType.FindSkipNavigation(navigationName);

					if (navigationBase != null && !(navigationBase is INavigation navigation && navigation.ForeignKey.IsOwnership))
					{
						this.loader.Load(invocation.Proxy, navigationName);
					}
				}
				else if (methodName.StartsWith("set_", StringComparison.Ordinal))
				{
					var navigationName = methodName.Substring(4);
					var navigationBase = this.entityType.FindNavigation(navigationName) ??
						(INavigationBase)this.entityType.FindSkipNavigation(navigationName);

					if (navigationBase != null && !(navigationBase is INavigation navigation && navigation.ForeignKey.IsOwnership))
					{
						this.loader.Load(invocation.Proxy, navigationName);
					}
				}

				invocation.Proceed();
			}
		}
	}
#pragma warning restore EF1001 // Internal EF Core API usage.
}