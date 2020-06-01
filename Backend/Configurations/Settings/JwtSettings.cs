namespace PetProject.Web.API.Configurations.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public double ExpirationInMinutes { get; set; }
    }
}
