﻿using ApiPortal.Security.TenantService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApiPortal.Security
{
    public class TenantBuilder<T> where T : Tenant
    {
        private readonly IServiceCollection _services;

        public TenantBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Registrar la implementación de Resolución de Tenants
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithResolutionStrategy<V>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where V : class, ITenantResolutionStrategy
        {
            _services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantResolutionStrategy), typeof(V), lifetime));
            return this;
        }

        /// <summary>
        /// Registrar la implementación del Repositorio de Tenants
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithStore<V>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where V : class, ITenantStore<T>
        {
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantStore<T>), typeof(V), lifetime));
            return this;
        }
    }
}
