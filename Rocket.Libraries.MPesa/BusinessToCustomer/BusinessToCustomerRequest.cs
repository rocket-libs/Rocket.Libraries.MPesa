using System;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Numbers;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Strings;

namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public class BusinessToCustomerRequest
    {
        /// <summary>
        /// The username of the M-Pesa B2C account API operator. 
        /// REQUIRED.
        /// </summary>
        /// <value></value>
        [StringIsNonNullable("B2C Initiator Name")]
        public string InitiatorName { get; set; }

        /// <summary>
        /// This is the value obtained after encrypting the API initiator password.
        /// REQUIRED.
        /// </summary>
        [StringIsNonNullable("B2C Security Credential")]
        public string SecurityCredential { get; set;  }

        /// <summary>
        /// This is a unique command that specifies B2C transaction type.
        /// REQUIRED.
        /// </summary>
        [StringIsInSet(
            StringComparison.InvariantCulture,
            BusinessToCustomerCommandIds.BusinessPayment,
            BusinessToCustomerCommandIds.PromotionPayment,
            BusinessToCustomerCommandIds.SalaryPayment
        )]
        [StringIsNonNullable("B2C Command Id")]
        public string CommandID { get; set; }

        /// <summary>
        /// The amount of money being sent to the customer.
        /// REQUIRED.   
        /// </summary>
        [MinimumNumber(1,"B2C Amount To Be Sent")]
        public decimal Amount { get; set; }

        /// <summary>
        /// This is the B2C organization shortcode from which the money is to be sent.
        /// REQUIRED.
        /// </summary>
        [StringIsNonNullable("The B2C Organization Shortcode")]
        public int PartyA { get; set; }

        /// <summary>
        /// This is the customer mobile number  to receive the amount.
        /// The number should have the country code (254) WITHOUT the plus sign.
        /// REQUIRED.
        /// </summary>
        [MinimumNumber(10000,"B2C Recepient's Phone Number")]
        public long PartyB { get; set; }

        /// <summary>
        /// Any additional information to be associated with the transaction.
        /// REQUIRED.
        /// </summary>
        [StringIsNonNullable("B2C Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// This is the URL to be specified in your request that will be used by API Proxy to send notification incase the payment request is timed out while awaiting processing in the queue. 
        /// REQUIRED.
        /// </summary>
        [StringIsNonNullable("B2C QueueTimeOutURL")]
        public string QueueTimeOutURL { get; set; }

        /// <summary>
        /// This is the URL to be specified in your request that will be used by M-Pesa to send notification upon processing of the payment request.
        /// REQUIRED.
        /// </summary>
        /// <value></value>
        [StringIsNonNullable("B2C ResultURL")]
        public string ResultURL { get; set; }

        public string Occassion { get; set; }

    }
}