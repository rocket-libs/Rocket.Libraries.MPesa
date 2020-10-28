namespace Rocket.Libraries.MPesa
{
    public class Credentials
    {
        public string ConsumerKey { get; set; }

        public Credentials(string consumerKey, string consumerSecret, string passKey)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            PassKey = passKey;
        }

        public string ConsumerSecret { get; set; }

        public string PassKey { get; set; }
    }
}