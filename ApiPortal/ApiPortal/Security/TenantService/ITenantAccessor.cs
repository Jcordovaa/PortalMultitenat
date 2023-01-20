namespace ApiPortal.Security.TenantService
{
    public interface ITenantAccessor<T> where T : Tenant
    {
        public T? Tenant { get; init; }
    }
}
