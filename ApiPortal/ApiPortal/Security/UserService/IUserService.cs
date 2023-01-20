namespace ApiPortal.Security.UserService
{
    public interface IUserService
    {
        string GetEmail();
        string GetRole();
        string GetTenant();
    }
}
