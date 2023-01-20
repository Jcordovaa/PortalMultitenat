namespace ApiPortal.ModelSoftland
{
    public class TipoDocSoftlandDTO
    {
        public string CodDoc { get; set; }
        public string DesDoc { get; set; }
    }

    public class TipoDocSoftlandAPIDTO
    {
        public string CodDoc { get; set; }
        public string DesDoc { get; set; }
        public string CtlUbi { get; set; }
        public string LibDoc { get; set; }
        public string LibRet { get; set; }
        public double ValorRet { get; set; }
        public string LibOpe { get; set; }
        public string DupIng { get; set; }
        public string Duplic { get; set; }
        public string Ingreso { get; set; }
        public string GenLetra { get; set; }
        public string IMPUMONTO { get; set; }
        public string CtaCteBan { get; set; }
        public string DupTDNumFec { get; set; }
        public string AplicaLote { get; set; }
        public string CtaContDef { get; set; }
        public int? DTEDocSII { get; set; }
        public string DupSMis { get; set; }
        public int? LCVSII { get; set; }
        public string DTEDocSIIDef { get; set; }
        public string AvisoCont { get; set; }
        public int DiasAviso { get; set; }
        public string TipoValidacionAviso { get; set; }
        public string ImgDocto { get; set; }
        public string OrigenImgDocto { get; set; }
        public string FormatoImgDocto { get; set; }
        public string ColorImgDocto { get; set; }
        public int ResImgDocto { get; set; }
        public string ManejaInfAdiDoc { get; set; }
        public string ControlaAuxV { get; set; }
        public string dispositivoCHK { get; set; }
        public int? IDdispositivoCHK { get; set; }
    }
}
