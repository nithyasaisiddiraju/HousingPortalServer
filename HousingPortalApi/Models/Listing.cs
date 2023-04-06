namespace HousingPortalApi.Models
{
    public class Listing
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}