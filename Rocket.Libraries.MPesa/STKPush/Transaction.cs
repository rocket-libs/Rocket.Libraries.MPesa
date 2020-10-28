namespace Rocket.Libraries.MPesa.STKPush
{
    public class Transaction
    {
        public Transaction(long requesterPhoneNumber, long businessShortCode, long amount, string transactionType)
        {
            RequesterPhoneNumber = requesterPhoneNumber;
            BusinessShortCode = businessShortCode;
            Amount = amount;
            TransactionType = transactionType;
        }

        public long RequesterPhoneNumber { get; set; }

        public long BusinessShortCode { get; set; }

        public long Amount { get; set; }

        public string AccountReference { get; set; }

        public string Description { get; set; }

        public string TransactionType { get; set; }

    }
}