using System;
using Newtonsoft.Json;

namespace ChatTelegramBotService.Model
{    
    public class Authentication
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("expires_in")]
        public int expires_in { get; set; }

        [JsonProperty("scope")]
        public string scope { get; set; }

        [JsonProperty("jti")]
        public string jti { get; set; }

    }    
}
