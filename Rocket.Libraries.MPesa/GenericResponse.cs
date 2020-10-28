using Newtonsoft.Json;

namespace Rocket.Libraries.MPesa
{
    public class GenericResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; } 

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; } 

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; } 
    }
}