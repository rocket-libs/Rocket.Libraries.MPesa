using System;
using System.Text;
using Newtonsoft.Json;

namespace Rocket.Libraries.MPesa.STKPush
{
    public class LipaNaMpesaOnline
    {
        private string timestamp;

        
        /// <summary>
        /// This is organizations shortcode (Paybill or Buygoods - A 5 to 6 digit account number) 
        /// used to identify an organization and receive the transaction.
        /// </summary>
        [JsonProperty("BusinessShortCode")]
        public string BusinessShortCode { get; set; }

        /// <summary>
        /// This is the Timestamp of the transaction, 
        /// normaly in the formart of YEAR+MONTH+DATE+HOUR+MINUTE+SECOND (YYYYMMDDHHMMSS) 
        /// Each part should be atleast two digits apart from the year which takes four digits.        
        /// </summary>
        [JsonProperty("Timestamp")]
        public string Timestamp 
        {
            get
            {
                if(string.IsNullOrEmpty(timestamp))
                {
                    timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                return timestamp;
            }
        }

        /// <summary>
        /// This is the transaction type that is used to identify the transaction when sending the request to M-Pesa. 
        /// The transaction type for M-Pesa Express is "CustomerPayBillOnline" 
        /// </summary>
        [JsonProperty("TransactionType")]
        public string TransactionType { get; set; }

        /// <summary>
        /// This is the Amount transacted, normally a numeric value. Money that customer pays to the Shorcode. 
        /// Only whole numbers are supported.
        /// </summary>
        [JsonProperty("Amount")]
        public string Amount { get; set; }

        /// <summary>
        /// The phone number sending money. The parameter expected is a Valid Safaricom Mobile Number 
        /// that is M-Pesa registered in the format 2547XXXXXXXX
        /// </summary>
        [JsonProperty("PartyA")]
        public string PartyA => PhoneNumber;

        /// <summary>
        /// The organization receiving the funds. The parameter expected is a 5 to 6 digit.
        /// This can be the same as BusinessShortCode value.
        /// </summary>
        [JsonProperty("PartyB")]
        public string PartyB => BusinessShortCode;

        /// <summary>
        /// The Mobile Number to receive the STK Pin Prompt. 
        /// This number can be the same as PartyA value.
        /// </summary>
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// A CallBack URL is a valid secure URL that is used to receive notifications from M-Pesa API. 
        /// It is the endpoint to which the results will be sent by M-Pesa API.
        /// </summary>
        [JsonProperty("CallBackURL")]
        public string CallBackURL { get; set; }

        /// <summary>
        /// Account Reference: This is an Alpha-Numeric parameter that is defined by your system as an Identifier 
        /// of the transaction for CustomerPayBillOnline transaction type. Along with the business name, 
        /// this value is also displayed to the customer in the STK PIN Prompt message. 
        /// Maximum of 12 characters.
        /// </summary>
        [JsonProperty("AccountReference")]
        public string AccountReference { get; set; }

        /// <summary>
        /// This is any additional information/comment that can be sent along with the request from your system. 
        /// Maximum of 13 Characters.
        /// </summary>
        [JsonProperty("TransactionDesc")]
        public string TransactionDesc { get; set; }

        /// <summary>
        /// Lipa Na Mpesa Online PassKey
        /// Provide the Passkey only if you want MpesaLib to Encode the Password for you.
        /// </summary>
        public string Passkey { get; set; }

        /// <summary>
        /// This is the password used for encrypting the request sent: A base64 encoded string. 
        /// The base64 string is a combination of Shortcode+Passkey+Timestamp
        /// The Defualt value is set by a private method that creates the necessary base64 encoded string
        /// Don't set this property if you have set the passKey property.
        /// </summary>
        [JsonProperty("Password")]
        public string Password => CalculatePassword(BusinessShortCode, Passkey, Timestamp);

        /// <summary>
        /// This method creates the necessary base64 encoded string that encrypts the request sent 
        /// </summary>
        private string CalculatePassword(string shortCode, string passkey, string timestamp)
        {
            return Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(shortCode + passkey + timestamp));
        }
    }
}
