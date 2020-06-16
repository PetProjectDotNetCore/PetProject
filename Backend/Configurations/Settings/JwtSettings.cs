namespace PetProject.Web.API.Configurations.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public double AccessTokenExpirationInMinutes { get; set; }
        public double RefreshTokenExpirationInMinutes { get; set; }
    }
}
