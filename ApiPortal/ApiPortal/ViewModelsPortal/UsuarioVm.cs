namespace ApiPortal.ViewModelsPortal
{
    public class UsuarioVm
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Avatar { get; set; }
        public int? IdRol { get; set; }
        public int? IdEmpresa { get; set; }
    }
}
