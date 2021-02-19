using Newtonsoft.Json;

namespace API.Models
{
    public class AuthModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
