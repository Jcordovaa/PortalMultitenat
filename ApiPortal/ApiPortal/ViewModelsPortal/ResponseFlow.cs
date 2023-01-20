namespace ApiPortal.ViewModelsPortal
{
    public class ResponseFlow
    {
        public string url { get; set; }
        public string token { get; set; }
        public int flowOrder { get; set; }
        public string commerceOrder { get; set; }
        public string requestDate { get; set; }
        public int status { get; set; }
        public string subject { get; set; }
        public string currency { get; set; }
        public float amount { get; set; }
        public string payer { get; set; }
        public PaymentData paymentData { get; set; }
        public string merchantId { get; set; }
    }

    public class PaymentData
    {
        public string date { get; set; }
        public string media { get; set; }
        public string conversionDate { get; set; }
        public float amount { get; set; }
        public string currency { get; set; }
        public string fee { get; set; }
        public float balance { get; set; }
        public string transferDate { get; set; }
    }
}
