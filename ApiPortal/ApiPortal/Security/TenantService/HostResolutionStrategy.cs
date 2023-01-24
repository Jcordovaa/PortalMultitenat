namespace ApiPortal.Security.TenantService
{
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly HttpContext? _httpContext;

        public HostResolutionStrategy(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
        }
        public async Task<string> GetTenantIdentifierAsync()
        {
            if (_httpContext is null)
            {
                return string.Empty;
            }

            //return await Task.FromResult(_httpContext.Request.Host.Host);
            var uri = new Uri(_httpContext.Request.Headers.Origin);
            return await Task.FromResult(uri.Host);
        }
    }
}
