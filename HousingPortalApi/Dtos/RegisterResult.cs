namespace HousingPortalApi.Dtos
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}