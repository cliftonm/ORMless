using Clifton;

namespace Models.Responses
{
    public class LoginResponse
    {
        [MapperProperty(Name = "AccessToken")]
        public string access_token { get; set; }

        [MapperProperty(Name = "RefreshToken")]
        public string refresh_token { get; set; }

        [MapperProperty(Name = "ExpiresIn")]
        public int expires_in { get; set; }

        [MapperProperty(Name = "ExpiresOn")]
        public long expires_on { get; set; }

        public string token_type { get; set; } = "Bearer";
    }
}
