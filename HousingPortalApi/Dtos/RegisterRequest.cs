namespace HousingPortalApi.Dtos
{
    public class RegisterRequest
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
        public string email { get; set; } = null!;
        public string phone { get; set; } = null!;
        public string major { get; set; } = null!;
        public int graduationYear { get; set; }

    }
}