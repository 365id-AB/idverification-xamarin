using System;
using Newtonsoft.Json;

namespace IdVerificationSampleApp.Models
{
    public class AccessTokenRequestDTO
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}