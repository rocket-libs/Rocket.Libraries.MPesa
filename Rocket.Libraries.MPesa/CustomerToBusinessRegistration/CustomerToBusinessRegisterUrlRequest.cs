using System;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Numbers;
using Rocket.Libraries.FormValidationHelper.Attributes.InBuilt.Strings;

namespace Rocket.Libraries.MPesa.CustomerToBusinessRegistration
{
    public class CustomerToBusinessRegisterUrlRequest
    {
        /// <summary>
        /// This is the URL that receives the validation request from API upon payment submission. The validation URL is only called if the external validation on the registered shortcode is enabled.
        /// REQUIRED
        /// </summary>
        [StringIsNonNullable("C2B ValidationURL")]
        public string ValidationURL { get; set; }

        /// <summary>
        /// This is the URL that receives the confirmation request from API upon payment completion.
        /// REQUIRED 
        /// </summary>
        [StringIsNonNullable("C2B ConfirmationURL")]
        public string ConfirmationURL { get; set; }


        /// <summary>
        /// This parameter specifies what is to happen if for any reason the validation URL is nor reachable. Note that, This is the default action value that determines what MPesa will do in the scenario that your endpoint is unreachable or is unable to respond on time. Only two values are allowed: Completed or Cancelled. Completed means MPesa will automatically complete your transaction, whereas Cancelled means MPesa will automatically cancel the transaction, in the event MPesa is unable to reach your Validation URL.
        /// REQUIRED
        /// </summary>
        [StringIsInSet(
            StringComparison.InvariantCulture,
            CustomerToBusinessResponseTypes.Canceled,
            CustomerToBusinessResponseTypes.Completed
        )]
        [StringIsNonNullable("C2B ResponseType")]
        public string ResponseType { get; set; }


        /// <summary>
        /// The short code of the organization. 
        /// </summary>
        [MinimumNumber(10000,"C2B Shortcode")]
        public int ShortCode { get; set; }
    }
}