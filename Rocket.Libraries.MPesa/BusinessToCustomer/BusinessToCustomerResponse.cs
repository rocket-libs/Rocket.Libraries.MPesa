namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public class BusinessToCustomerResponse
    {
        public string OriginatorConversationId { get; set; }
        public string ConversationId { get; set; }

        public string ResponseDescription { get; set; }

        public object Result { get; set; }

        public string ResultDesc { get; set; }

        public int ResultType { get; set; }

        public int ResultCode { get; set; }

        public bool Succeeded => ResultCode == 0;

        public string TransactionID { get; set; }

        public object ResultParameters { get; set; }

        public object ResultParameter { get; set; }

        public string TransactionReceipt { get; set; }

        public string TransactionAmount { get; set; }

        public decimal B2CWorkingAccountAvailableFunds { get; set; }

        public decimal B2CUtilityAccountAvailableFunds { get; set; }

        public string TransactionCompletedDateTime { get; set; }

        public string ReceiverPartyPublicName { get; set; }

        public decimal B2CChargesPaidAccountAvailableFunds { get; set; }

        public string B2CRecipientIsRegisteredCustomer { get; set; }

    }
}