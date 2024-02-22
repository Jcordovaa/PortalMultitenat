using ApiPortal.Dal.Models_Admin;
using ApiPortal.Services;
using System;

namespace ApiPortal.Security.TenantService
{
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly HttpContext? _httpContext;
        private readonly PortalAdministracionSoftlandContext _admin;

        public HostResolutionStrategy(IHttpContextAccessor httpContext, PortalAdministracionSoftlandContext admin)
        {
            _httpContext = httpContext.HttpContext;
            _admin = admin;
        }
        public async Task<string> GetTenantIdentifierAsync()
        {
            if (_httpContext is null)
            {
                return string.Empty;
            }

            //return await Task.FromResult(_httpContext.Request.Host.Host);
            if(_httpContext.Request.Path.Value == "/api/ProcesaPagos/GeneraPagoElectronico" || _httpContext.Request.Path.Value == "/api/Softland/CallbackPago")
            {
                
                string tenant = Encrypt.Base64Decode(_httpContext.Request.QueryString.Value.Split("tenant").Last().Remove(0, 1));
                return await Task.FromResult(tenant);
            }
            else
            {
                if (!string.IsNullOrEmpty(_httpContext.Request.Headers.Origin.ToString()))
                {
                    var uri = new Uri(_httpContext.Request.Headers.Origin);
                    return await Task.FromResult(uri.Host);
                }
                else
                {
                   var tenantDefecto = _admin.Tenants.FirstOrDefault();
                    return await Task.FromResult(tenantDefecto.Identifier);

                }
             
            }
           
            
        }
    }
}
