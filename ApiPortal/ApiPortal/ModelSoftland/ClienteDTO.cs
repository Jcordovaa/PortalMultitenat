namespace ApiPortal.ModelSoftland
{
    public class ClienteDTO
    {
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? EncriptadoCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public List<ContactoDTO>? Contactos { get; set; }
        public string? ComCod { get; set; }
        public string? ComunaNombre { get; set; }
        public int? IdRegion { get; set; }
        public string? RegionNombre { get; set; }
        public string? DirAux { get; set; }
        public string? DirNum { get; set; }
        public string? Telefono { get; set; }
        public int? Estado { get; set; }
        public string? EmailDTE { get; set; }
        public string? EsReceptorDTE { get; set; }
        public string? NomGiro { get; set; }
        public string? CodGiro { get; set; }
        public int? IdCliente { get; set; }
        public string? CodVendedor { get; set; }
        public string? CodCondVenta { get; set; }
        public string? CodCatCliente { get; set; }
        public string? CodLista { get; set; }
        public string? CodCargo { get; set; }
        public string? CodCobrador { get; set; }
        public string? CodCanalVenta { get; set; }
        public string? CorreoUsuario { get; set; }
        public List<ClienteSaldosDTO>? Documentos { get; set; }
        public double? TotalSaldo { get; set; }
        public double? TotalDocumentos { get; set; }
        public int? CantidadDocumentos { get; set; }
        public bool? EnviarTodosContactos { get; set; }
        public bool? EnviarFicha { get; set; }
    }
    public class ClienteAPIDTO
    {
        public string? Total { get; set; }
        public string? CantidadPorPagina { get; set; }
        public string? PaginaActual { get; set; }
        public string? CodAux { get; set; }
        public string? NomAux { get; set; }
        public string? RutAux { get; set; }
        public string? Giraux { get; set; }
        public string? GirDes { get; set; }
        public string? ComCod { get; set; }
        public string? ComDes { get; set; }
        public string? CiuCod { get; set; }
        public string? CiuDes { get; set; }
        public int? Id_Region { get; set; }
        public string? RegionDes { get; set; }
        public string? PaiCod { get; set; }
        public string? PaiDes { get; set; }
        public string? DirAux { get; set; }
        public string? DirNum { get; set; }
        public string? FonAux1 { get; set; }
        public string? Bloqueado { get; set; }
        public string? EMail { get; set; }
        public string? EsReceptorDTE { get; set; }
        public string? EMailDTE { get; set; }
        public List<DireccionesDespachoAsociadaAPIDTO>? DireccionesDespachoAsociadas { get; set; }
        public string? CodVen { get; set; }
        public string? CodLista { get; set; }
        public string? Codcob { get; set; }
        public string? ConVta { get; set; }
        public string? catcli { get; set; }
        public string? CodCargo { get; set; }
        public bool? EnviarTodosContactos { get; set; }
        public bool? EnviarFicha { get; set; }
        public string? correoUsuario { get; set; }
        public int? AccesoEnviado { get; set; }

    }

    public class DireccionesDespachoAsociadaAPIDTO
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Contacto { get; set; }
        public string CiuDes { get; set; }
        public string CiuCod { get; set; }
        public string ComDes { get; set; }
        public string ComCod { get; set; }
        public string PaiDes { get; set; }
        public string PaiCod { get; set; }
    }
}
