using ApiPortal.Dal.Models_Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace ApiPortal.Security.TenantService
{
    public class DbContextTenantStore : ITenantStore<Tenant>
    {
        private readonly PortalAdministracionSoftlandContext _context;
        private readonly IMemoryCache _cache;

        public DbContextTenantStore(PortalAdministracionSoftlandContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Tenant> GetTenantAsync(string identifier)
        {
            var cacheKey = $"Cache_{identifier}";
            var tenant = _cache.Get<Tenant>(cacheKey);


            if (tenant is null)
            {
                var entity = await _context.Tenants
                    .FirstOrDefaultAsync(q => q.Identifier == identifier)
                        ?? throw new ArgumentException($"identifier no es un tenant válido: " + identifier);

                tenant = new Tenant(entity.IdTenant, entity.Identifier);

                //tenant.Items["Name"] = entity.RazonSocial;
                tenant.Items["ConnectionString"] = entity.ConnectionString;

                _cache.Set(cacheKey, tenant);
            }

            return tenant;
        }
    }
}
