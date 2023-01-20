namespace ApiPortal.ViewModelsPortal
{
    public class AccesosVm
    {
        public int IdAcceso { get; set; }
        public string Nombre { get; set; }
        public int? MenuPadre { get; set; }
        public int? Activo { get; set; }
        public bool Checked { get; set; }
        public int TotalFilas { get; set; }

        public virtual ICollection<PermisosVm> Permisos { get; set; }
    }
}
