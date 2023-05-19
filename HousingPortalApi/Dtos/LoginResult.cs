namespace HousingPortalApi.Dtos
{
    public class LoginResult
    {
        public bool success { get; set; }
        public string message { get; set; } = null!;
        public string token { get; set; }
    }
}
