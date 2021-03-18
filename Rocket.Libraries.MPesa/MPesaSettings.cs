namespace Rocket.Libraries.MPesa
{
    public class MPesaSettings
    {
        public string Environment { get; set; }

        public int HttpRetries { get; set; }

        public string StkCallBackUrl { get; set; }
    }
}