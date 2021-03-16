namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public class BusinessToCustomerRequest
    {
        
        public string InitiatorName { get; set; }

        
        public string SecurityCredential { get; set;  }

        public string CommandID { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// This is the B2C organization shortcode from which the money is to be sent.
        /// </summary>
        public int PartyA { get; set; }

        /// <summary>
        /// This is the customer mobile number  to receive the amount.
        /// The number should have the country code (254) WITHOUT the plus sign.
        /// </summary>
        public long PartyB { get; set; }

        public string Remarks { get; set; }

        public string QueueTimeOutURL { get; set; }

        public string ResultURL { get; set; }

        public string Occassion { get; set; }

    }
}