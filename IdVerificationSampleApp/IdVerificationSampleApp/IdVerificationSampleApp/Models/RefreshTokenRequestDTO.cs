using System;
using Newtonsoft.Json;

namespace IdVerificationSampleApp.Models
{
    public class RefreshTokenRequestDTO
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}