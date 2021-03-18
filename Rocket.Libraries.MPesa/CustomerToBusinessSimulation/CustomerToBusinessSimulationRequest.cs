using System;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Numbers;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Strings;
using Rocket.Libraries.MPesa.STKPush;

namespace Rocket.Libraries.MPesa.CustomerToBusinessSimulation
{
    public class CustomerToBusinessSimulationRequest
    {

        /// <summary>
        /// This is a unique identifier of the transaction type: There are two types of these Identifiers:
        /// CustomerPayBillOnline and CustomerBuyGoodsOnline.
        /// REQUIRED
        /// </summary>
        [StringIsInSet(
            StringComparison.InvariantCulture,
            TransactionTypes.CustomerBuyGoodsOnline,
            TransactionTypes.CustomerPayBillOnline)]
        [StringIsNonNullable("C2B Command ID")]
        public string CommandID { get; set; }

        /// <summary>
        /// This is the amount being transacted. The parameter expected is a numeric value.
        /// REQUIRED
        /// </summary>
        [MinimumNumber(1,"C2B Amount being sent")]
        public decimal Amount { get; set; }

        /// <summary>
        /// This is the phone number initiating the C2B transaction.
        /// Should begin with country code and without a leading '+' sign.
        /// REQUIRED
        /// </summary>
        [StringIsValidPhoneNumberWithCountryCodeOrDefault("C2B Customer Phone Number",requireLeadingPlusSign: false)]
        public long Msisdn { get; set; }

        /// <summary>
        /// This is used on CustomerPayBillOnline option only. This is where a customer is expected to enter a unique bill identifier, e.g an Account Number. 
        /// </summary>
        public string BillRefNumber { get; set; }

        /// <summary>
        /// This is the Short Code receiving the amount being transacted.
        /// REQUIRED    
        /// </summary>
        [MinimumNumber(10000,"C2B ShortCode")]
        public long ShortCode { get; set; }

    }
}