namespace Rocket.Libraries.MPesa.CustomerToBusinessCallback
{
    public class CustomerToBusinessCallbackResponse
    {
        public string TransactionType { get; set; }

        public string TransID { get; set; }

        public string TransTime { get; set; }

        public decimal TransAmount { get; set; }

        public string BusinessShortCode { get; set; }

        public string BillRefNumber { get; set; }

        public decimal OrgAccountBalance { get; set; }

        public long MSISDN { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string ResponseDescription { get; set; }
    }
}