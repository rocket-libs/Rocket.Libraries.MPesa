namespace Rocket.Libraries.MPesa.HttpClients
{
    public enum HttpClientTypes : byte
    {
        TokenFetcher = 1,
        STKPusher = 2,

        GenericClient = 3
    }
}