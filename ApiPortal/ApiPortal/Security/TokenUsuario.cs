namespace ApiPortal.Security
{
    public class TokenUsuario
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
