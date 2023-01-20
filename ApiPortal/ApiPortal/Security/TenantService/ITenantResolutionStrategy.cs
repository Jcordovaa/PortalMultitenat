namespace ApiPortal.Security.TenantService
{
    public interface ITenantResolutionStrategy
    {
        Task<string> GetTenantIdentifierAsync();
    }
}
