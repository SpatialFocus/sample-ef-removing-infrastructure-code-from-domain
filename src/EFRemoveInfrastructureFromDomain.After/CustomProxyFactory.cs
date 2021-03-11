// <copyright file="CustomProxyFactory.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFRemoveInfrastructureFromDomain.After
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using Castle.DynamicProxy;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.EntityFrameworkCore.Internal;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Proxies.Internal;
	using IInterceptor = Castle.DynamicProxy.IInterceptor;

#pragma warning disable EF1001 // Internal EF Core API usage.
	public class CustomProxyFactory : IProxyFactory
	{
		private static readonly Type notifyPropertyChangedInterface = typeof(INotifyPropertyChanged);
		private static readonly Type notifyPropertyChangingInterface = typeof(INotifyPropertyChanging);
		private static readonly Type proxyLazyLoaderInterface = typeof(IProxyLazyLoader);

		private readonly ProxyGenerator generator = new();

		public virtual object Create(DbContext context, Type type, params object[] constructorArguments)
		{
			IEntityType? entityType = context.Model.FindRuntimeEntityType(type);

			if (entityType == null)
			{
				if (context.Model.IsShared(type))
				{
					throw new InvalidOperationException(ProxiesStrings.EntityTypeNotFoundShared(type.ShortDisplayName()));
				}

				throw new InvalidOperationException(CoreStrings.EntityTypeNotFound(type.ShortDisplayName()));
			}

			return CreateProxy(context, (IEntityType)entityType, constructorArguments);
		}

		public virtual object CreateLazyLoadingProxy(DbContext context, IEntityType entityType, ILazyLoader loader,
			object[] constructorArguments)
		{
			ProxiesOptionsExtension? options = context.GetService<IDbContextOptions>().FindExtension<ProxiesOptionsExtension>();

			if (options == null)
			{
				throw new InvalidOperationException(ProxiesStrings.ProxyServicesMissing);
			}

			return CreateLazyLoadingProxy(options, entityType, context.GetService<ILazyLoader>(), constructorArguments);
		}

		public virtual object CreateProxy(DbContext context, IEntityType entityType, object[] constructorArguments)
		{
			ProxiesOptionsExtension? options = context.GetService<IDbContextOptions>().FindExtension<ProxiesOptionsExtension>();

			if (options == null)
			{
				throw new InvalidOperationException(ProxiesStrings.ProxyServicesMissing);
			}

			if (options.UseLazyLoadingProxies)
			{
				return CreateLazyLoadingProxy(options, entityType, context.GetService<ILazyLoader>(), constructorArguments);
			}

			return CreateProxy(options, entityType, constructorArguments);
		}

		public virtual Type CreateProxyType(ProxiesOptionsExtension options, IEntityType entityType) =>
			this.generator.ProxyBuilder.CreateClassProxyType(entityType.ClrType, GetInterfacesToProxy(options, entityType),
				ProxyGenerationOptions.Default);

		private object CreateLazyLoadingProxy(
			ProxiesOptionsExtension options, IEntityType entityType, ILazyLoader loader, object[] constructorArguments) =>
			this.generator.CreateClassProxy(entityType.ClrType, GetInterfacesToProxy(options, entityType), ProxyGenerationOptions.Default,
				constructorArguments,
				GetNotifyChangeInterceptors(options, entityType, new CustomLazyLoadingInterceptor(entityType, loader)));

		private object CreateProxy(ProxiesOptionsExtension options, IEntityType entityType, object[] constructorArguments) =>
			this.generator.CreateClassProxy(entityType.ClrType, GetInterfacesToProxy(options, entityType), ProxyGenerationOptions.Default,
				constructorArguments, GetNotifyChangeInterceptors(options, entityType));

		private Type[] GetInterfacesToProxy(ProxiesOptionsExtension options, IEntityType entityType)
		{
			List<Type>? interfacesToProxy = new List<Type>();

			if (options.UseLazyLoadingProxies)
			{
				interfacesToProxy.Add(CustomProxyFactory.proxyLazyLoaderInterface);
			}

			if (options.UseChangeTrackingProxies)
			{
				if (!CustomProxyFactory.notifyPropertyChangedInterface.IsAssignableFrom(entityType.ClrType))
				{
					interfacesToProxy.Add(CustomProxyFactory.notifyPropertyChangedInterface);
				}

				if (!CustomProxyFactory.notifyPropertyChangingInterface.IsAssignableFrom(entityType.ClrType))
				{
					interfacesToProxy.Add(CustomProxyFactory.notifyPropertyChangingInterface);
				}
			}

			return interfacesToProxy.ToArray();
		}

		private IInterceptor[] GetNotifyChangeInterceptors(ProxiesOptionsExtension options, IEntityType entityType,
			CustomLazyLoadingInterceptor? lazyLoadingInterceptor = null)
		{
			List<IInterceptor>? interceptors = new List<IInterceptor>();

			if (lazyLoadingInterceptor != null)
			{
				interceptors.Add(lazyLoadingInterceptor);
			}

			if (options.UseChangeTrackingProxies)
			{
				if (!CustomProxyFactory.notifyPropertyChangedInterface.IsAssignableFrom(entityType.ClrType))
				{
					interceptors.Add(new PropertyChangedInterceptor(entityType, options.CheckEquality));
				}

				if (!CustomProxyFactory.notifyPropertyChangingInterface.IsAssignableFrom(entityType.ClrType))
				{
					interceptors.Add(new PropertyChangingInterceptor(entityType, options.CheckEquality));
				}
			}

			return interceptors.ToArray();
		}
	}
#pragma warning restore EF1001 // Internal EF Core API usage.
}