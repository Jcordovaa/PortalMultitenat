namespace ApiPortal.ViewModelsPortal
{
    public class UsuariosVm
    {
        public int? IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Activo { get; set; }
        public int? IdPerfil { get; set; }
        public System.DateTime? FechaCreacion { get; set; }
        public System.DateTime? FechaEnvioValidacion { get; set; }
        public int? CuentaActivada { get; set; }
        public string? NombrePerfil { get; set; }
        public PerfilVm? Perfil { get; set; }
        public int? TotalFilas { get; set; }
    }
}
