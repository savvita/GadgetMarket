using System.Text.Json.Serialization;

namespace GadgetMarket.Model
{
    public class UserTokenModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("user_email")]
        public string UserEmail { get; set; } = null!;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;
    }
}
