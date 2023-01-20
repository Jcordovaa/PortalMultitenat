namespace ApiPortal.ModelSoftland
{
    public class ResumenContableAPIDTO
    {
        public double MontoAutorizado { get; set; }
        public double MontoUtilizado { get; set; }
        public double Disponible { get; set; }
        public string EstadoBloqueo { get; set; }
        public string EstadoSobregiro { get; set; }
    }
}
