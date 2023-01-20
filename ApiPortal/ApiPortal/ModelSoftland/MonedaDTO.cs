namespace ApiPortal.ModelSoftland
{
    public class MonedaDTO
    {
        public string CodMon { get; set; }
        public string DesMon { get; set; }
    }

    public class MonedaAPIDTO
    {
        public string CodMon { get; set; }
        public string DesMon { get; set; }
        public string SimMon { get; set; }
        public string CodEdi { get; set; }
        public string DecMon { get; set; }
        public string DecMonPre { get; set; }
        public string SepMiles { get; set; }
        public string SepDecimal { get; set; }
        public string UtilSepMiles { get; set; }
    }
}
