namespace ApiPortal.ViewModelsPortal
{
    public class PermisosVm
    {
        public int? IdPermiso { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdAcceso { get; set; }
        public int? Modificar { get; set; }
        public int? Consultar { get; set; }
        public int? Actualizar { get; set; }
        public int? Insertar { get; set; }

        public int? TotalFilas { get; set; }

        public bool? Checked { get; set; }

        public virtual PerfilVm? Perfil { get; set; }
        public virtual AccesosVm? Acceso { get; set; }
    }
}
