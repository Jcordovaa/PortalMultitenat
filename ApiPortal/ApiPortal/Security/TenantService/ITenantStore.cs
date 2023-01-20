namespace ApiPortal.Security.TenantService
{
    public interface ITenantStore<T> where T : Tenant
    {
        Task<T> GetTenantAsync(string identifier);
    }
}
